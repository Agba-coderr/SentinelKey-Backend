using Microsoft.AspNetCore.Mvc;
using SentinelKey.Api.Infrastructure;
using SentinelKey.Application.Organizations.CreateOrganization;
using SentinelKey.Contracts.Common;
using SentinelKey.Contracts.Organizations;

namespace SentinelKey.Api.Controllers;

[ApiController]
[Route("api/v1/organizations")]
public sealed class OrganizationsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OrganizationResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<OrganizationResponse>>> CreateOrganization(
        [FromBody] CreateOrganizationRequest request,
        [FromServices] IOrganizationOnboardingService onboardingService,
        CancellationToken cancellationToken)
    {
        var result = await onboardingService.CreateAsync(
            new CreateOrganizationCommand(
                request.Name,
                request.Code,
                request.PlatformName,
                request.CallbackBaseUrl,
                request.PushChallengesEnabled,
                request.TransactionChallengesEnabled,
                ActorId: "bootstrap-admin",
                CorrelationId: HttpContext.GetCorrelationId()),
            cancellationToken);

        return CreatedAtAction(
            nameof(CreateOrganization),
            new ApiResponse<OrganizationResponse>(
                true,
                "Organization created successfully.",
                result.Organization));
    }
}
