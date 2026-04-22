using Microsoft.EntityFrameworkCore;
using SentinelKey.Application.Common.Abstractions.Repositories;
using SentinelKey.Domain.Transactions;
using SentinelKey.Infrastructure.Persistence;

namespace SentinelKey.Infrastructure.Repositories;

public sealed class TransactionChallengeRepository : ITransactionChallengeRepository
{
    private readonly SentinelKeyDbContext _dbContext;

    public TransactionChallengeRepository(SentinelKeyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(TransactionChallenge challenge, CancellationToken cancellationToken)
    {
        await _dbContext.TransactionChallengeSet.AddAsync(challenge, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<TransactionChallenge?> GetByIdAsync(Guid challengeId, CancellationToken cancellationToken)
    {
        return _dbContext.TransactionChallengeSet.FirstOrDefaultAsync(x => x.Id == challengeId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<TransactionChallenge>> ListByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        return await _dbContext.TransactionChallengeSet
            .Where(x => x.OrganizationId == organizationId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);
    }

    public async Task UpdateAsync(TransactionChallenge challenge, CancellationToken cancellationToken)
    {
        _dbContext.TransactionChallengeSet.Update(challenge);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
