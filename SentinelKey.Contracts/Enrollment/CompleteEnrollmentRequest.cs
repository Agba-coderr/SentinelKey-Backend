namespace SentinelKey.Contracts.Enrollment;

public sealed record CompleteEnrollmentRequest(
    string DeviceIdentifier,
    string DeviceName,
    string Platform,
    string AppVersion);
