using System.Security.Cryptography;
using System.Text;
using SentinelKey.Application.Common.Abstractions.Repositories;
using SentinelKey.Application.Common.Abstractions.Security;
using SentinelKey.Contracts.Otp;
using SentinelKey.Domain.Auditing;

namespace SentinelKey.Application.Otp.ValidateOtp;

public sealed class OtpValidationService : IOtpValidationService
{
    private const int AllowedDriftWindows = 1;
    private readonly IOtpCredentialRepository _otpCredentialRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IOtpSecretProtector _otpSecretProtector;
    private readonly IReplayProtectionService _replayProtectionService;
    private readonly IRateLimitService _rateLimitService;

    public OtpValidationService(
        IOtpCredentialRepository otpCredentialRepository,
        IAuditLogRepository auditLogRepository,
        IOtpSecretProtector otpSecretProtector,
        IReplayProtectionService replayProtectionService,
        IRateLimitService rateLimitService)
    {
        _otpCredentialRepository = otpCredentialRepository;
        _auditLogRepository = auditLogRepository;
        _otpSecretProtector = otpSecretProtector;
        _replayProtectionService = replayProtectionService;
        _rateLimitService = rateLimitService;
    }

    public async Task<ValidateOtpResult> ValidateAsync(ValidateOtpCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.OtpCode))
        {
            throw new ArgumentException("OTP code is required.", nameof(command));
        }

        var credential = await _otpCredentialRepository.GetByDeviceAndUserAsync(
            command.DeviceId,
            command.UserId,
            cancellationToken);

        if (credential is null)
        {
            throw new InvalidOperationException("OTP credential was not found.");
        }

        var rateLimitKey = $"otp:{command.DeviceId}:{command.UserId}";
        var isAllowed = await _rateLimitService.IsAllowedAsync(
            rateLimitKey,
            maxAttempts: 5,
            window: TimeSpan.FromMinutes(1),
            cancellationToken);

        if (!isAllowed)
        {
            throw new InvalidOperationException("OTP validation rate limit exceeded.");
        }

        var rawSecret = _otpSecretProtector.Unprotect(credential.EncryptedSecret);
        var validatedAtUtc = DateTimeOffset.UtcNow;
        var unixTimeSeconds = validatedAtUtc.ToUnixTimeSeconds();
        var currentCounter = unixTimeSeconds / credential.PeriodSeconds;

        var matchedCounter = FindMatchingCounter(rawSecret, credential.Digits, currentCounter, command.OtpCode);
        if (matchedCounter is null)
        {
            await _rateLimitService.RegisterAttemptAsync(rateLimitKey, TimeSpan.FromMinutes(1), cancellationToken);
            await WriteAuditAsync(
                AuditActionType.OtpValidationFailed,
                command,
                $"OTP validation failed for device '{command.DeviceId}'.",
                cancellationToken);

            return new ValidateOtpResult(new OtpValidationResponse(
                false,
                "Rejected",
                command.DeviceId,
                command.UserId,
                validatedAtUtc));
        }

        var replayScope = $"otp-replay:{command.DeviceId}:{command.UserId}";
        var replayToken = $"{matchedCounter}:{command.OtpCode}";
        if (await _replayProtectionService.HasSeenAsync(replayScope, replayToken, cancellationToken))
        {
            await _rateLimitService.RegisterAttemptAsync(rateLimitKey, TimeSpan.FromMinutes(1), cancellationToken);
            await WriteAuditAsync(
                AuditActionType.OtpValidationFailed,
                command,
                $"OTP replay detected for device '{command.DeviceId}'.",
                cancellationToken);

            return new ValidateOtpResult(new OtpValidationResponse(
                false,
                "ReplayDetected",
                command.DeviceId,
                command.UserId,
                validatedAtUtc));
        }

        await _replayProtectionService.MarkAsSeenAsync(
            replayScope,
            replayToken,
            TimeSpan.FromSeconds(credential.PeriodSeconds * 2),
            cancellationToken);

        await _rateLimitService.ResetAsync(rateLimitKey, cancellationToken);
        await WriteAuditAsync(
            AuditActionType.OtpValidated,
            command,
            $"OTP validated successfully for device '{command.DeviceId}'.",
            cancellationToken);

        return new ValidateOtpResult(new OtpValidationResponse(
            true,
            "Validated",
            command.DeviceId,
            command.UserId,
            validatedAtUtc));
    }

    private async Task WriteAuditAsync(
        string actionType,
        ValidateOtpCommand command,
        string description,
        CancellationToken cancellationToken)
    {
        await _auditLogRepository.AddAsync(
            new AuditLog(
                actionType,
                command.ActorId,
                "mobile-device",
                command.DeviceId.ToString(),
                "OtpCredential",
                description,
                command.CorrelationId),
            cancellationToken);
    }

    private static long? FindMatchingCounter(string secret, int digits, long currentCounter, string providedCode)
    {
        for (var offset = -AllowedDriftWindows; offset <= AllowedDriftWindows; offset++)
        {
            var counter = currentCounter + offset;
            if (counter < 0)
            {
                continue;
            }

            var generatedCode = GenerateTotp(secret, digits, counter);
            if (generatedCode == providedCode)
            {
                return counter;
            }
        }

        return null;
    }

    private static string GenerateTotp(string secret, int digits, long counter)
    {
        var key = Base32Decode(secret);
        Span<byte> counterBytes = stackalloc byte[8];
        for (var index = 7; index >= 0; index--)
        {
            counterBytes[index] = (byte)(counter & 0xFF);
            counter >>= 8;
        }

        using var hmac = new HMACSHA1(key);
        var hash = hmac.ComputeHash(counterBytes.ToArray());
        var offset = hash[^1] & 0x0F;
        var binaryCode =
            ((hash[offset] & 0x7F) << 24) |
            ((hash[offset + 1] & 0xFF) << 16) |
            ((hash[offset + 2] & 0xFF) << 8) |
            (hash[offset + 3] & 0xFF);

        var otp = binaryCode % (int)Math.Pow(10, digits);
        return otp.ToString(new string('0', digits));
    }

    private static byte[] Base32Decode(string input)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var cleaned = input.Trim().TrimEnd('=').ToUpperInvariant();
        var output = new List<byte>();
        var buffer = 0;
        var bitsLeft = 0;

        foreach (var character in cleaned)
        {
            var value = alphabet.IndexOf(character);
            if (value < 0)
            {
                throw new InvalidOperationException("OTP secret contains invalid Base32 characters.");
            }

            buffer = (buffer << 5) | value;
            bitsLeft += 5;

            if (bitsLeft >= 8)
            {
                output.Add((byte)((buffer >> (bitsLeft - 8)) & 0xFF));
                bitsLeft -= 8;
            }
        }

        return output.ToArray();
    }
}
