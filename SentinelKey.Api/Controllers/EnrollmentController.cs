using Microsoft.AspNetCore.Mvc;
using SentinelKey.Api.Infrastructure;
using SentinelKey.Application.Enrollment.CompleteEnrollment;
using SentinelKey.Application.Enrollment.CreateEnrollmentSession;
using SentinelKey.Contracts.Common;
using SentinelKey.Contracts.Devices;
using SentinelKey.Contracts.Enrollment;

namespace SentinelKey.Api.Controllers;

[ApiController]
[Route("api/v1")]
public sealed class EnrollmentController : ControllerBase
{
    [HttpPost("organizations/{organizationId:guid}/enrollments")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentSessionResponse>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<EnrollmentSessionResponse>>> CreateEnrollmentSession(
        Guid organizationId,
        [FromBody] CreateEnrollmentSessionRequest request,
        [FromServices] IEnrollmentService enrollmentService,
        CancellationToken cancellationToken)
    {
        var result = await enrollmentService.CreateAsync(
            new CreateEnrollmentSessionCommand(
                organizationId,
                request.UserId,
                request.ExpiresInMinutes,
                "bootstrap-admin",
                HttpContext.GetCorrelationId()),
            cancellationToken);

        return CreatedAtAction(
            nameof(CreateEnrollmentSession),
            new ApiResponse<EnrollmentSessionResponse>(
                true,
                "Enrollment session created successfully.",
                result.EnrollmentSession));
    }

    [HttpPost("enrollments/{enrollmentSessionId:guid}/complete")]
    [ProducesResponseType(typeof(ApiResponse<DeviceResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<DeviceResponse>>> CompleteEnrollment(
        Guid enrollmentSessionId,
        [FromBody] CompleteEnrollmentRequest request,
        [FromServices] IEnrollmentService enrollmentService,
        CancellationToken cancellationToken)
    {
        var result = await enrollmentService.CompleteAsync(
            new CompleteEnrollmentCommand(
                enrollmentSessionId,
                request.DeviceIdentifier,
                request.DeviceName,
                request.Platform,
                request.AppVersion,
                request.DeviceIdentifier,
                HttpContext.GetCorrelationId()),
            cancellationToken);

        return Ok(new ApiResponse<DeviceResponse>(
            true,
            "Enrollment completed successfully.",
            result.Device));
    }
}
