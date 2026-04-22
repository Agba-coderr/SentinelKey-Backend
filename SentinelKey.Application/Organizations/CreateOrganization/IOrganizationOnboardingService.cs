namespace SentinelKey.Application.Organizations.CreateOrganization;

public interface IOrganizationOnboardingService
{
    Task<CreateOrganizationResult> CreateAsync(CreateOrganizationCommand command, CancellationToken cancellationToken);
}
