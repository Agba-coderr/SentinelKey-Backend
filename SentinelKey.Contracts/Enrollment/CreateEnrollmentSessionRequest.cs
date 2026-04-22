namespace SentinelKey.Contracts.Enrollment;

public sealed record CreateEnrollmentSessionRequest(
    string UserId,
    int ExpiresInMinutes = 10);
