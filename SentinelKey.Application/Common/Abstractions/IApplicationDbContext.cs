using SentinelKey.Domain.Auditing;
using SentinelKey.Domain.Organizations;

namespace SentinelKey.Application.Common.Abstractions;

public interface IApplicationDbContext
{
    IQueryable<Organization> Organizations { get; }
    IQueryable<AuditLog> AuditLogs { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
