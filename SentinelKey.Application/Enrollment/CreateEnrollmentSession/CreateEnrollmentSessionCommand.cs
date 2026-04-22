namespace SentinelKey.Application.Enrollment.CreateEnrollmentSession;

public sealed record CreateEnrollmentSessionCommand(
    Guid OrganizationId,
    string UserId,
    int ExpiresInMinutes,
    string ActorId,
    string CorrelationId);
