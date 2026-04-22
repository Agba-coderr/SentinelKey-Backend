using SentinelKey.Application.Common.Abstractions.Repositories;
using SentinelKey.Domain.Auditing;
using SentinelKey.Infrastructure.Persistence;

namespace SentinelKey.Infrastructure.Repositories;

public sealed class AuditLogRepository : IAuditLogRepository
{
    private readonly SentinelKeyDbContext _dbContext;

    public AuditLogRepository(SentinelKeyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken)
    {
        await _dbContext.AuditLogSet.AddAsync(auditLog, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
