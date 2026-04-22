namespace SentinelKey.Application.Enrollment.CompleteEnrollment;

public sealed record CompleteEnrollmentCommand(
    Guid EnrollmentSessionId,
    string DeviceIdentifier,
    string DeviceName,
    string Platform,
    string AppVersion,
    string ActorId,
    string CorrelationId);
