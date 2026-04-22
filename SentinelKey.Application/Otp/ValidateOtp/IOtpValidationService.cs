namespace SentinelKey.Application.Otp.ValidateOtp;

public interface IOtpValidationService
{
    Task<ValidateOtpResult> ValidateAsync(ValidateOtpCommand command, CancellationToken cancellationToken);
}
