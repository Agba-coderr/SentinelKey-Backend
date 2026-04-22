namespace SentinelKey.Application.Otp.ValidateOtp;

public sealed record ValidateOtpCommand(
    Guid DeviceId,
    string UserId,
    string OtpCode,
    string ActorId,
    string CorrelationId);
