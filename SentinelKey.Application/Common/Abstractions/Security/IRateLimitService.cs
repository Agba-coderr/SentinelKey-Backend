namespace SentinelKey.Application.Common.Abstractions.Security;

public interface IRateLimitService
{
    Task<bool> IsAllowedAsync(string key, int maxAttempts, TimeSpan window, CancellationToken cancellationToken);
    Task RegisterAttemptAsync(string key, TimeSpan window, CancellationToken cancellationToken);
    Task ResetAsync(string key, CancellationToken cancellationToken);
}
