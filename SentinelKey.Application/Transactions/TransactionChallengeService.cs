using SentinelKey.Application.Common.Abstractions.Repositories;
using SentinelKey.Application.Transactions.CreateTransactionChallenge;
using SentinelKey.Application.Transactions.DecideTransactionChallenge;
using SentinelKey.Contracts.Transactions;
using SentinelKey.Domain.Auditing;
using SentinelKey.Domain.Organizations;
using SentinelKey.Domain.Transactions;

namespace SentinelKey.Application.Transactions;

public sealed class TransactionChallengeService : ITransactionChallengeService
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ITransactionChallengeRepository _transactionChallengeRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public TransactionChallengeService(
        IOrganizationRepository organizationRepository,
        ITransactionChallengeRepository transactionChallengeRepository,
        IAuditLogRepository auditLogRepository)
    {
        _organizationRepository = organizationRepository;
        _transactionChallengeRepository = transactionChallengeRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<CreateTransactionChallengeResult> CreateAsync(
        CreateTransactionChallengeCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Amount <= 0)
        {
            throw new ArgumentException("Transaction amount must be greater than zero.", nameof(command));
        }

        var organization = await _organizationRepository.GetByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            throw new InvalidOperationException("Organization was not found.");
        }

        var challenge = new TransactionChallenge(
            command.OrganizationId,
            command.UserId,
            command.ExternalTransactionId,
            command.Amount,
            command.Currency,
            command.MerchantName,
            command.Reason,
            DateTimeOffset.UtcNow.AddMinutes(command.ExpiresInMinutes));

        await _transactionChallengeRepository.AddAsync(challenge, cancellationToken);
        await _auditLogRepository.AddAsync(
            new AuditLog(
                AuditActionType.TransactionChallengeCreated,
                command.ActorId,
                "gapsentinel",
                challenge.Id.ToString(),
                nameof(TransactionChallenge),
                $"Transaction challenge created for external transaction '{challenge.ExternalTransactionId}'.",
                command.CorrelationId),
            cancellationToken);

        return new CreateTransactionChallengeResult(ToResponse(challenge));
    }

    public async Task<DecideTransactionChallengeResult> DecideAsync(
        DecideTransactionChallengeCommand command,
        CancellationToken cancellationToken)
    {
        var challenge = await _transactionChallengeRepository.GetByIdAsync(command.ChallengeId, cancellationToken);
        if (challenge is null)
        {
            throw new InvalidOperationException("Transaction challenge was not found.");
        }

        if (command.Approve)
        {
            challenge.Approve(command.DecisionNote);
        }
        else
        {
            challenge.Reject(command.DecisionNote);
        }

        await _transactionChallengeRepository.UpdateAsync(challenge, cancellationToken);
        await _auditLogRepository.AddAsync(
            new AuditLog(
                command.Approve
                    ? AuditActionType.TransactionChallengeApproved
                    : AuditActionType.TransactionChallengeRejected,
                command.ActorId,
                "mobile-device",
                challenge.Id.ToString(),
                nameof(TransactionChallenge),
                $"Transaction challenge '{challenge.ExternalTransactionId}' was {(command.Approve ? "approved" : "rejected")}.",
                command.CorrelationId),
            cancellationToken);

        return new DecideTransactionChallengeResult(ToResponse(challenge));
    }

    public async Task<IReadOnlyCollection<TransactionChallengeResponse>> ListByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        var challenges = await _transactionChallengeRepository.ListByOrganizationAsync(organizationId, cancellationToken);
        return challenges.Select(ToResponse).ToArray();
    }

    private static TransactionChallengeResponse ToResponse(TransactionChallenge challenge)
    {
        return new TransactionChallengeResponse(
            challenge.Id,
            challenge.OrganizationId,
            challenge.UserId,
            challenge.ExternalTransactionId,
            challenge.Amount,
            challenge.Currency,
            challenge.MerchantName,
            challenge.Reason,
            challenge.Status.ToString(),
            challenge.ExpiresAtUtc,
            challenge.CreatedAtUtc,
            challenge.DecidedAtUtc,
            challenge.DecisionNote);
    }
}
