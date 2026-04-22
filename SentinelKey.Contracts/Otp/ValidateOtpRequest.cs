namespace SentinelKey.Contracts.Otp;

public sealed record ValidateOtpRequest(
    Guid DeviceId,
    string UserId,
    string OtpCode);
