using SentinelKey.Domain.Auditing;

namespace SentinelKey.Application.Common.Abstractions.Repositories;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken);
}
