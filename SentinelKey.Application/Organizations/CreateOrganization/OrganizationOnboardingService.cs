using SentinelKey.Application.Common.Abstractions.Repositories;
using SentinelKey.Contracts.Organizations;
using SentinelKey.Domain.Auditing;
using SentinelKey.Domain.Organizations;

namespace SentinelKey.Application.Organizations.CreateOrganization;

public sealed class OrganizationOnboardingService : IOrganizationOnboardingService
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public OrganizationOnboardingService(
        IOrganizationRepository organizationRepository,
        IAuditLogRepository auditLogRepository)
    {
        _organizationRepository = organizationRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<CreateOrganizationResult> CreateAsync(
        CreateOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            throw new ArgumentException("Organization name is required.", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(command.Code))
        {
            throw new ArgumentException("Organization code is required.", nameof(command));
        }

        if (await _organizationRepository.ExistsByCodeAsync(command.Code, cancellationToken))
        {
            throw new InvalidOperationException($"Organization code '{command.Code}' already exists.");
        }

        var organization = new Organization(command.Name, command.Code);
        var platformConfiguration = organization.AddPlatformConfiguration(
            command.PlatformName,
            command.CallbackBaseUrl,
            command.PushChallengesEnabled,
            command.TransactionChallengesEnabled);

        await _organizationRepository.AddAsync(organization, cancellationToken);

        await _auditLogRepository.AddAsync(
            new AuditLog(
                AuditActionType.OrganizationCreated,
                command.ActorId,
                "api-client",
                organization.Id.ToString(),
                nameof(Organization),
                $"Organization '{organization.Name}' was created.",
                command.CorrelationId),
            cancellationToken);

        await _auditLogRepository.AddAsync(
            new AuditLog(
                AuditActionType.PlatformConfigurationCreated,
                command.ActorId,
                "api-client",
                platformConfiguration.Id.ToString(),
                nameof(PlatformConfiguration),
                $"Platform '{platformConfiguration.PlatformName}' was configured for organization '{organization.Name}'.",
                command.CorrelationId),
            cancellationToken);

        var response = new OrganizationResponse(
            organization.Id,
            organization.Name,
            organization.Code,
            organization.Status.ToString(),
            organization.CreatedAtUtc,
            [
                new PlatformConfigurationResponse(
                    platformConfiguration.Id,
                    platformConfiguration.PlatformName,
                    platformConfiguration.CallbackBaseUrl,
                    platformConfiguration.PushChallengesEnabled,
                    platformConfiguration.TransactionChallengesEnabled)
            ]);

        return new CreateOrganizationResult(response);
    }
}
