using SentinelKey.Application.Transactions.CreateTransactionChallenge;
using SentinelKey.Application.Transactions.DecideTransactionChallenge;
using SentinelKey.Contracts.Transactions;

namespace SentinelKey.Application.Transactions;

public interface ITransactionChallengeService
{
    Task<CreateTransactionChallengeResult> CreateAsync(CreateTransactionChallengeCommand command, CancellationToken cancellationToken);
    Task<DecideTransactionChallengeResult> DecideAsync(DecideTransactionChallengeCommand command, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<TransactionChallengeResponse>> ListByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken);
}
