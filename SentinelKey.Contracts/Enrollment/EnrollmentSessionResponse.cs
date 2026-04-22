namespace SentinelKey.Contracts.Enrollment;

public sealed record EnrollmentSessionResponse(
    Guid Id,
    Guid OrganizationId,
    string UserId,
    string QrCodePayload,
    string Status,
    DateTimeOffset ExpiresAtUtc,
    DateTimeOffset CreatedAtUtc);
