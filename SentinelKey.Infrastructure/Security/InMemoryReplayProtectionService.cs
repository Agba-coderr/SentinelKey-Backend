using Microsoft.Extensions.Caching.Memory;
using SentinelKey.Application.Common.Abstractions.Security;

namespace SentinelKey.Infrastructure.Security;

public sealed class InMemoryReplayProtectionService : IReplayProtectionService
{
    private readonly IMemoryCache _memoryCache;

    public InMemoryReplayProtectionService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<bool> HasSeenAsync(string scope, string token, CancellationToken cancellationToken)
    {
        return Task.FromResult(_memoryCache.TryGetValue($"{scope}:{token}", out _));
    }

    public Task MarkAsSeenAsync(string scope, string token, TimeSpan ttl, CancellationToken cancellationToken)
    {
        _memoryCache.Set($"{scope}:{token}", true, ttl);
        return Task.CompletedTask;
    }
}
