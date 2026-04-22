using Microsoft.Extensions.Caching.Memory;
using SentinelKey.Application.Common.Abstractions.Security;

namespace SentinelKey.Infrastructure.Security;

public sealed class InMemoryRateLimitService : IRateLimitService
{
    private sealed record AttemptCounter(int Count, DateTimeOffset ExpiresAtUtc);

    private readonly IMemoryCache _memoryCache;

    public InMemoryRateLimitService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<bool> IsAllowedAsync(string key, int maxAttempts, TimeSpan window, CancellationToken cancellationToken)
    {
        if (!_memoryCache.TryGetValue<AttemptCounter>(key, out var counter))
        {
            return Task.FromResult(true);
        }

        if (counter!.ExpiresAtUtc <= DateTimeOffset.UtcNow)
        {
            _memoryCache.Remove(key);
            return Task.FromResult(true);
        }

        return Task.FromResult(counter.Count < maxAttempts);
    }

    public Task RegisterAttemptAsync(string key, TimeSpan window, CancellationToken cancellationToken)
    {
        var expiresAtUtc = DateTimeOffset.UtcNow.Add(window);
        if (_memoryCache.TryGetValue<AttemptCounter>(key, out var counter) && counter!.ExpiresAtUtc > DateTimeOffset.UtcNow)
        {
            _memoryCache.Set(key, counter with { Count = counter.Count + 1 }, counter.ExpiresAtUtc);
            return Task.CompletedTask;
        }

        _memoryCache.Set(key, new AttemptCounter(1, expiresAtUtc), expiresAtUtc);
        return Task.CompletedTask;
    }

    public Task ResetAsync(string key, CancellationToken cancellationToken)
    {
        _memoryCache.Remove(key);
        return Task.CompletedTask;
    }
}
