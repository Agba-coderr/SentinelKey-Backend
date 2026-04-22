namespace SentinelKey.Api.Infrastructure;

public static class HttpContextExtensions
{
    public static string GetCorrelationId(this HttpContext httpContext)
    {
        return httpContext.Items[CorrelationIdMiddleware.HeaderName]?.ToString()
            ?? Guid.NewGuid().ToString("N");
    }
}
