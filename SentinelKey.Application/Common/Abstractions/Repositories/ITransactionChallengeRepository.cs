using SentinelKey.Domain.Transactions;

namespace SentinelKey.Application.Common.Abstractions.Repositories;

public interface ITransactionChallengeRepository
{
    Task AddAsync(TransactionChallenge challenge, CancellationToken cancellationToken);
    Task<TransactionChallenge?> GetByIdAsync(Guid challengeId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<TransactionChallenge>> ListByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken);
    Task UpdateAsync(TransactionChallenge challenge, CancellationToken cancellationToken);
}
