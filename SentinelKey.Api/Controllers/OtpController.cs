using Microsoft.AspNetCore.Mvc;
using SentinelKey.Api.Infrastructure;
using SentinelKey.Application.Otp.ValidateOtp;
using SentinelKey.Contracts.Common;
using SentinelKey.Contracts.Otp;

namespace SentinelKey.Api.Controllers;

[ApiController]
[Route("api/v1/otp")]
public sealed class OtpController : ControllerBase
{
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ApiResponse<OtpValidationResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<OtpValidationResponse>>> ValidateOtp(
        [FromBody] ValidateOtpRequest request,
        [FromServices] IOtpValidationService otpValidationService,
        CancellationToken cancellationToken)
    {
        var result = await otpValidationService.ValidateAsync(
            new ValidateOtpCommand(
                request.DeviceId,
                request.UserId,
                request.OtpCode,
                request.DeviceId.ToString(),
                HttpContext.GetCorrelationId()),
            cancellationToken);

        return Ok(new ApiResponse<OtpValidationResponse>(
            result.Validation.IsValid,
            result.Validation.IsValid
                ? "OTP validated successfully."
                : "OTP validation was rejected.",
            result.Validation));
    }
}
