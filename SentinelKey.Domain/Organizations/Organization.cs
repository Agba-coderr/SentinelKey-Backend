using SentinelKey.Domain.Common;

namespace SentinelKey.Domain.Organizations;

public sealed class Organization : Entity
{
    private readonly List<PlatformConfiguration> _platformConfigurations = [];

    private Organization()
    {
    }

    public Organization(string name, string code)
    {
        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        Status = OrganizationStatus.Active;
    }

    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public OrganizationStatus Status { get; private set; }
    public IReadOnlyCollection<PlatformConfiguration> PlatformConfigurations => _platformConfigurations;

    public PlatformConfiguration AddPlatformConfiguration(
        string platformName,
        string callbackBaseUrl,
        bool pushChallengesEnabled,
        bool transactionChallengesEnabled)
    {
        var configuration = new PlatformConfiguration(
            Id,
            platformName,
            callbackBaseUrl,
            pushChallengesEnabled,
            transactionChallengesEnabled);

        _platformConfigurations.Add(configuration);
        Touch();
        return configuration;
    }

    public void Suspend()
    {
        Status = OrganizationStatus.Suspended;
        Touch();
    }
}
