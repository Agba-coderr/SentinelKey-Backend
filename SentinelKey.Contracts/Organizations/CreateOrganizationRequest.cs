namespace SentinelKey.Contracts.Organizations;

public sealed record CreateOrganizationRequest(
    string Name,
    string Code,
    string PlatformName,
    string CallbackBaseUrl,
    bool PushChallengesEnabled,
    bool TransactionChallengesEnabled);
