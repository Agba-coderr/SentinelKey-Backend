namespace SentinelKey.Contracts.Devices;

public sealed record DeviceResponse(
    Guid Id,
    Guid OrganizationId,
    string UserId,
    string DeviceIdentifier,
    string DeviceName,
    string Platform,
    string AppVersion,
    string Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? BoundAtUtc,
    DateTimeOffset? SuspendedAtUtc);
