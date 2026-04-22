using Microsoft.EntityFrameworkCore;
using SentinelKey.Application.Common.Abstractions.Repositories;
using SentinelKey.Domain.Enrollment;
using SentinelKey.Infrastructure.Persistence;

namespace SentinelKey.Infrastructure.Repositories;

public sealed class EnrollmentRepository : IEnrollmentRepository
{
    private readonly SentinelKeyDbContext _dbContext;

    public EnrollmentRepository(SentinelKeyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(EnrollmentSession enrollmentSession, CancellationToken cancellationToken)
    {
        await _dbContext.EnrollmentSessionSet.AddAsync(enrollmentSession, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<EnrollmentSession?> GetByIdAsync(Guid enrollmentSessionId, CancellationToken cancellationToken)
    {
        return _dbContext.EnrollmentSessionSet.FirstOrDefaultAsync(x => x.Id == enrollmentSessionId, cancellationToken);
    }

    public async Task UpdateAsync(EnrollmentSession enrollmentSession, CancellationToken cancellationToken)
    {
        _dbContext.EnrollmentSessionSet.Update(enrollmentSession);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
