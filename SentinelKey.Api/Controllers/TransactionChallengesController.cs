using Microsoft.AspNetCore.Mvc;
using SentinelKey.Api.Infrastructure;
using SentinelKey.Application.Transactions;
using SentinelKey.Application.Transactions.CreateTransactionChallenge;
using SentinelKey.Application.Transactions.DecideTransactionChallenge;
using SentinelKey.Contracts.Common;
using SentinelKey.Contracts.Transactions;

namespace SentinelKey.Api.Controllers;

[ApiController]
[Route("api/v1")]
public sealed class TransactionChallengesController : ControllerBase
{
    [HttpPost("organizations/{organizationId:guid}/transaction-challenges")]
    [ProducesResponseType(typeof(ApiResponse<TransactionChallengeResponse>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<TransactionChallengeResponse>>> CreateChallenge(
        Guid organizationId,
        [FromBody] CreateTransactionChallengeRequest request,
        [FromServices] ITransactionChallengeService transactionChallengeService,
        CancellationToken cancellationToken)
    {
        var result = await transactionChallengeService.CreateAsync(
            new CreateTransactionChallengeCommand(
                organizationId,
                request.UserId,
                request.ExternalTransactionId,
                request.Amount,
                request.Currency,
                request.MerchantName,
                request.Reason,
                request.ExpiresInMinutes,
                "gapsentinel",
                HttpContext.GetCorrelationId()),
            cancellationToken);

        return CreatedAtAction(
            nameof(CreateChallenge),
            new ApiResponse<TransactionChallengeResponse>(
                true,
                "Transaction challenge created successfully.",
                result.Challenge));
    }

    [HttpGet("organizations/{organizationId:guid}/transaction-challenges")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<TransactionChallengeResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<TransactionChallengeResponse>>>> ListChallenges(
        Guid organizationId,
        [FromServices] ITransactionChallengeService transactionChallengeService,
        CancellationToken cancellationToken)
    {
        var result = await transactionChallengeService.ListByOrganizationAsync(organizationId, cancellationToken);

        return Ok(new ApiResponse<IReadOnlyCollection<TransactionChallengeResponse>>(
            true,
            "Transaction challenges retrieved successfully.",
            result));
    }

    [HttpPost("transaction-challenges/{challengeId:guid}/approve")]
    [ProducesResponseType(typeof(ApiResponse<TransactionChallengeResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<TransactionChallengeResponse>>> ApproveChallenge(
        Guid challengeId,
        [FromBody] DecideTransactionChallengeRequest request,
        [FromServices] ITransactionChallengeService transactionChallengeService,
        CancellationToken cancellationToken)
    {
        var result = await transactionChallengeService.DecideAsync(
            new DecideTransactionChallengeCommand(
                challengeId,
                true,
                request.DecisionNote,
                "mobile-device",
                HttpContext.GetCorrelationId()),
            cancellationToken);

        return Ok(new ApiResponse<TransactionChallengeResponse>(
            true,
            "Transaction challenge approved successfully.",
            result.Challenge));
    }

    [HttpPost("transaction-challenges/{challengeId:guid}/reject")]
    [ProducesResponseType(typeof(ApiResponse<TransactionChallengeResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<TransactionChallengeResponse>>> RejectChallenge(
        Guid challengeId,
        [FromBody] DecideTransactionChallengeRequest request,
        [FromServices] ITransactionChallengeService transactionChallengeService,
        CancellationToken cancellationToken)
    {
        var result = await transactionChallengeService.DecideAsync(
            new DecideTransactionChallengeCommand(
                challengeId,
                false,
                request.DecisionNote,
                "mobile-device",
                HttpContext.GetCorrelationId()),
            cancellationToken);

        return Ok(new ApiResponse<TransactionChallengeResponse>(
            true,
            "Transaction challenge rejected successfully.",
            result.Challenge));
    }
}
