namespace SentinelKey.Contracts.Otp;

public sealed record OtpValidationResponse(
    bool IsValid,
    string Status,
    Guid DeviceId,
    string UserId,
    DateTimeOffset ValidatedAtUtc);
