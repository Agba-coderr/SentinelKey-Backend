using Microsoft.EntityFrameworkCore;
using SentinelKey.Application.Common.Abstractions.Repositories;
using SentinelKey.Domain.Organizations;
using SentinelKey.Infrastructure.Persistence;

namespace SentinelKey.Infrastructure.Repositories;

public sealed class OrganizationRepository : IOrganizationRepository
{
    private readonly SentinelKeyDbContext _dbContext;

    public OrganizationRepository(SentinelKeyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Organization organization, CancellationToken cancellationToken)
    {
        await _dbContext.OrganizationSet.AddAsync(organization, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Organization?> GetByIdAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        return _dbContext.OrganizationSet
            .Include(x => x.PlatformConfigurations)
            .FirstOrDefaultAsync(x => x.Id == organizationId, cancellationToken);
    }

    public Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return _dbContext.OrganizationSet.AnyAsync(x => x.Code == normalizedCode, cancellationToken);
    }
}
