namespace SentinelKey.Application.Organizations.CreateOrganization;

public sealed record CreateOrganizationCommand(
    string Name,
    string Code,
    string PlatformName,
    string CallbackBaseUrl,
    bool PushChallengesEnabled,
    bool TransactionChallengesEnabled,
    string ActorId,
    string CorrelationId);
