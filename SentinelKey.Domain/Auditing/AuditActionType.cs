namespace SentinelKey.Domain.Auditing;

public static class AuditActionType
{
    public const string OrganizationCreated = "organization.created";
    public const string PlatformConfigurationCreated = "platform.configuration.created";
    public const string EnrollmentSessionCreated = "enrollment.session.created";
    public const string EnrollmentCompleted = "enrollment.completed";
    public const string DeviceBound = "device.bound";
    public const string DeviceSuspended = "device.suspended";
    public const string OtpValidated = "otp.validated";
    public const string OtpValidationFailed = "otp.validation.failed";
    public const string TransactionChallengeCreated = "transaction.challenge.created";
    public const string TransactionChallengeApproved = "transaction.challenge.approved";
    public const string TransactionChallengeRejected = "transaction.challenge.rejected";
}
