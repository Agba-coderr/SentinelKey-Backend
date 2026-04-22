using SentinelKey.Domain.Enrollment;

namespace SentinelKey.Application.Common.Abstractions.Repositories;

public interface IEnrollmentRepository
{
    Task AddAsync(EnrollmentSession enrollmentSession, CancellationToken cancellationToken);
    Task<EnrollmentSession?> GetByIdAsync(Guid enrollmentSessionId, CancellationToken cancellationToken);
    Task UpdateAsync(EnrollmentSession enrollmentSession, CancellationToken cancellationToken);
}
