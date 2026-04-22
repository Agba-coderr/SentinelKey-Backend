namespace SentinelKey.Contracts.Organizations;

public sealed record OrganizationResponse(
    Guid Id,
    string Name,
    string Code,
    string Status,
    DateTimeOffset CreatedAtUtc,
    IReadOnlyCollection<PlatformConfigurationResponse> Platforms);

public sealed record PlatformConfigurationResponse(
    Guid Id,
    string PlatformName,
    string CallbackBaseUrl,
    bool PushChallengesEnabled,
    bool TransactionChallengesEnabled);
