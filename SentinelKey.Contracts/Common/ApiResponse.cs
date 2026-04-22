namespace SentinelKey.Contracts.Common;

public sealed record ApiResponse<T>(bool Success, string Message, T Data);
