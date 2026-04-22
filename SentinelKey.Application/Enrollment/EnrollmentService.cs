using SentinelKey.Application.Common.Abstractions.Repositories;
using SentinelKey.Application.Common.Abstractions.Security;
using SentinelKey.Application.Enrollment.CompleteEnrollment;
using SentinelKey.Application.Enrollment.CreateEnrollmentSession;
using SentinelKey.Contracts.Devices;
using SentinelKey.Contracts.Enrollment;
using SentinelKey.Domain.Auditing;
using SentinelKey.Domain.Devices;
using SentinelKey.Domain.Enrollment;
using SentinelKey.Domain.Otp;

namespace SentinelKey.Application.Enrollment;

public sealed class EnrollmentService : IEnrollmentService
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly IOtpCredentialRepository _otpCredentialRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IOtpSecretProtector _otpSecretProtector;

    public EnrollmentService(
        IOrganizationRepository organizationRepository,
        IEnrollmentRepository enrollmentRepository,
        IDeviceRepository deviceRepository,
        IOtpCredentialRepository otpCredentialRepository,
        IAuditLogRepository auditLogRepository,
        IOtpSecretProtector otpSecretProtector)
    {
        _organizationRepository = organizationRepository;
        _enrollmentRepository = enrollmentRepository;
        _deviceRepository = deviceRepository;
        _otpCredentialRepository = otpCredentialRepository;
        _auditLogRepository = auditLogRepository;
        _otpSecretProtector = otpSecretProtector;
    }

    public async Task<CreateEnrollmentSessionResult> CreateAsync(
        CreateEnrollmentSessionCommand command,
        CancellationToken cancellationToken)
    {
        if (command.ExpiresInMinutes <= 0)
        {
            throw new ArgumentException("Enrollment session expiry must be greater than zero.", nameof(command));
        }

        var organization = await _organizationRepository.GetByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            throw new InvalidOperationException("Organization was not found.");
        }

        var expiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(command.ExpiresInMinutes);
        var payload = $"sentinelkey://enroll?org={organization.Code}&user={Uri.EscapeDataString(command.UserId)}&session={Guid.NewGuid():N}";

        var session = new EnrollmentSession(
            organization.Id,
            command.UserId,
            payload,
            expiresAtUtc);

        await _enrollmentRepository.AddAsync(session, cancellationToken);

        await _auditLogRepository.AddAsync(
            new AuditLog(
                AuditActionType.EnrollmentSessionCreated,
                command.ActorId,
                "api-client",
                session.Id.ToString(),
                nameof(EnrollmentSession),
                $"Enrollment session created for user '{command.UserId}' in organization '{organization.Name}'.",
                command.CorrelationId),
            cancellationToken);

        return new CreateEnrollmentSessionResult(
            new EnrollmentSessionResponse(
                session.Id,
                session.OrganizationId,
                session.UserId,
                session.QrCodePayload,
                session.Status.ToString(),
                session.ExpiresAtUtc,
                session.CreatedAtUtc));
    }

    public async Task<CompleteEnrollmentResult> CompleteAsync(
        CompleteEnrollmentCommand command,
        CancellationToken cancellationToken)
    {
        var session = await _enrollmentRepository.GetByIdAsync(command.EnrollmentSessionId, cancellationToken);
        if (session is null)
        {
            throw new InvalidOperationException("Enrollment session was not found.");
        }

        var device = new Device(
            session.OrganizationId,
            session.UserId,
            command.DeviceIdentifier,
            command.DeviceName,
            command.Platform,
            command.AppVersion);

        device.Activate();
        await _deviceRepository.AddAsync(device, cancellationToken);

        var rawSecret = _otpSecretProtector.GenerateSecret();
        var encryptedSecret = _otpSecretProtector.Protect(rawSecret);
        var otpCredential = new OtpCredential(
            session.OrganizationId,
            device.Id,
            session.UserId,
            encryptedSecret,
            "TOTP-SHA1",
            6,
            30);

        await _otpCredentialRepository.AddAsync(otpCredential, cancellationToken);

        session.Complete(device.Id);
        await _enrollmentRepository.UpdateAsync(session, cancellationToken);

        await _auditLogRepository.AddAsync(
            new AuditLog(
                AuditActionType.EnrollmentCompleted,
                command.ActorId,
                "mobile-device",
                session.Id.ToString(),
                nameof(EnrollmentSession),
                $"Enrollment session for user '{session.UserId}' was completed.",
                command.CorrelationId),
            cancellationToken);

        await _auditLogRepository.AddAsync(
            new AuditLog(
                AuditActionType.DeviceBound,
                command.ActorId,
                "mobile-device",
                device.Id.ToString(),
                nameof(Device),
                $"Device '{device.DeviceName}' was bound to user '{device.UserId}'.",
                command.CorrelationId),
            cancellationToken);

        return new CompleteEnrollmentResult(ToDeviceResponse(device));
    }

    private static DeviceResponse ToDeviceResponse(Device device)
    {
        return new DeviceResponse(
            device.Id,
            device.OrganizationId,
            device.UserId,
            device.DeviceIdentifier,
            device.DeviceName,
            device.Platform,
            device.AppVersion,
            device.Status.ToString(),
            device.CreatedAtUtc,
            device.BoundAtUtc,
            device.SuspendedAtUtc);
    }
}
