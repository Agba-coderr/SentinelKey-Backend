using SentinelKey.Application.Enrollment.CompleteEnrollment;

namespace SentinelKey.Application.Enrollment.CreateEnrollmentSession;

public interface IEnrollmentService
{
    Task<CreateEnrollmentSessionResult> CreateAsync(CreateEnrollmentSessionCommand command, CancellationToken cancellationToken);
    Task<CompleteEnrollmentResult> CompleteAsync(CompleteEnrollmentCommand command, CancellationToken cancellationToken);
}
