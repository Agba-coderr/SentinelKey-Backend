using SentinelKey.Domain.Common;

namespace SentinelKey.Domain.Otp;

public sealed class OtpCredential : Entity
{
    private OtpCredential()
    {
    }

    public OtpCredential(
        Guid organizationId,
        Guid deviceId,
        string userId,
        string encryptedSecret,
        string algorithm,
        int digits,
        int periodSeconds)
    {
        OrganizationId = organizationId;
        DeviceId = deviceId;
        UserId = userId.Trim();
        EncryptedSecret = encryptedSecret.Trim();
        Algorithm = algorithm.Trim();
        Digits = digits;
        PeriodSeconds = periodSeconds;
    }

    public Guid OrganizationId { get; private set; }
    public Guid DeviceId { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string EncryptedSecret { get; private set; } = string.Empty;
    public string Algorithm { get; private set; } = string.Empty;
    public int Digits { get; private set; }
    public int PeriodSeconds { get; private set; }
}
