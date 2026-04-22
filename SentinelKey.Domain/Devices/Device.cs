using SentinelKey.Domain.Common;

namespace SentinelKey.Domain.Devices;

public sealed class Device : Entity
{
    private Device()
    {
    }

    public Device(
        Guid organizationId,
        string userId,
        string deviceIdentifier,
        string deviceName,
        string platform,
        string appVersion)
    {
        OrganizationId = organizationId;
        UserId = userId.Trim();
        DeviceIdentifier = deviceIdentifier.Trim();
        DeviceName = deviceName.Trim();
        Platform = platform.Trim();
        AppVersion = appVersion.Trim();
        Status = DeviceStatus.Pending;
    }

    public Guid OrganizationId { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string DeviceIdentifier { get; private set; } = string.Empty;
    public string DeviceName { get; private set; } = string.Empty;
    public string Platform { get; private set; } = string.Empty;
    public string AppVersion { get; private set; } = string.Empty;
    public DeviceStatus Status { get; private set; }
    public DateTimeOffset? BoundAtUtc { get; private set; }
    public DateTimeOffset? SuspendedAtUtc { get; private set; }
    public DateTimeOffset? RevokedAtUtc { get; private set; }

    public void Activate()
    {
        Status = DeviceStatus.Active;
        BoundAtUtc = DateTimeOffset.UtcNow;
        SuspendedAtUtc = null;
        RevokedAtUtc = null;
        Touch();
    }

    public void Suspend()
    {
        Status = DeviceStatus.Suspended;
        SuspendedAtUtc = DateTimeOffset.UtcNow;
        Touch();
    }

    public void Revoke()
    {
        Status = DeviceStatus.Revoked;
        RevokedAtUtc = DateTimeOffset.UtcNow;
        Touch();
    }
}
