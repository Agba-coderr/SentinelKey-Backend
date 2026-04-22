using SentinelKey.Domain.Common;

namespace SentinelKey.Domain.Enrollment;

public sealed class EnrollmentSession : Entity
{
    private EnrollmentSession()
    {
    }

    public EnrollmentSession(
        Guid organizationId,
        string userId,
        string qrCodePayload,
        DateTimeOffset expiresAtUtc)
    {
        OrganizationId = organizationId;
        UserId = userId.Trim();
        QrCodePayload = qrCodePayload.Trim();
        ExpiresAtUtc = expiresAtUtc;
        Status = EnrollmentStatus.Pending;
    }

    public Guid OrganizationId { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string QrCodePayload { get; private set; } = string.Empty;
    public EnrollmentStatus Status { get; private set; }
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public DateTimeOffset? CompletedAtUtc { get; private set; }
    public Guid? DeviceId { get; private set; }

    public void Complete(Guid deviceId)
    {
        if (Status != EnrollmentStatus.Pending)
        {
            throw new InvalidOperationException("Only pending enrollment sessions can be completed.");
        }

        if (ExpiresAtUtc <= DateTimeOffset.UtcNow)
        {
            throw new InvalidOperationException("Enrollment session has expired.");
        }

        Status = EnrollmentStatus.Completed;
        CompletedAtUtc = DateTimeOffset.UtcNow;
        DeviceId = deviceId;
        Touch();
    }
}
