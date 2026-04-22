using Microsoft.AspNetCore.Mvc;
using SentinelKey.Api.Infrastructure;
using SentinelKey.Application.Devices.DeviceManagement;
using SentinelKey.Contracts.Common;
using SentinelKey.Contracts.Devices;

namespace SentinelKey.Api.Controllers;

[ApiController]
[Route("api/v1")]
public sealed class DevicesController : ControllerBase
{
    [HttpGet("organizations/{organizationId:guid}/devices")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<DeviceResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<DeviceResponse>>>> ListOrganizationDevices(
        Guid organizationId,
        [FromServices] IDeviceManagementService deviceManagementService,
        CancellationToken cancellationToken)
    {
        var result = await deviceManagementService.ListByOrganizationAsync(organizationId, cancellationToken);

        return Ok(new ApiResponse<IReadOnlyCollection<DeviceResponse>>(
            true,
            "Devices retrieved successfully.",
            result));
    }

    [HttpPost("devices/{deviceId:guid}/suspend")]
    [ProducesResponseType(typeof(ApiResponse<DeviceResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<DeviceResponse>>> SuspendDevice(
        Guid deviceId,
        [FromServices] IDeviceManagementService deviceManagementService,
        CancellationToken cancellationToken)
    {
        var result = await deviceManagementService.SuspendAsync(
            deviceId,
            "bootstrap-admin",
            HttpContext.GetCorrelationId(),
            cancellationToken);

        return Ok(new ApiResponse<DeviceResponse>(
            true,
            "Device suspended successfully.",
            result));
    }
}
