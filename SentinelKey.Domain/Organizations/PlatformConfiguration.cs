using SentinelKey.Domain.Common;

namespace SentinelKey.Domain.Organizations;

public sealed class PlatformConfiguration : Entity
{
    private PlatformConfiguration()
    {
    }

    public PlatformConfiguration(
        Guid organizationId,
        string platformName,
        string callbackBaseUrl,
        bool pushChallengesEnabled,
        bool transactionChallengesEnabled)
    {
        OrganizationId = organizationId;
        PlatformName = platformName.Trim();
        CallbackBaseUrl = callbackBaseUrl.Trim();
        PushChallengesEnabled = pushChallengesEnabled;
        TransactionChallengesEnabled = transactionChallengesEnabled;
    }

    public Guid OrganizationId { get; private set; }
    public string PlatformName { get; private set; } = string.Empty;
    public string CallbackBaseUrl { get; private set; } = string.Empty;
    public bool PushChallengesEnabled { get; private set; }
    public bool TransactionChallengesEnabled { get; private set; }
}
