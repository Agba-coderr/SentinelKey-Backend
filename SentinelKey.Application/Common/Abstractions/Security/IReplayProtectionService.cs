namespace SentinelKey.Application.Common.Abstractions.Security;

public interface IReplayProtectionService
{
    Task<bool> HasSeenAsync(string scope, string token, CancellationToken cancellationToken);
    Task MarkAsSeenAsync(string scope, string token, TimeSpan ttl, CancellationToken cancellationToken);
}
