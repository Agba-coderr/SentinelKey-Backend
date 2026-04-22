namespace SentinelKey.Contracts.Transactions;

public sealed record TransactionChallengeResponse(
    Guid Id,
    Guid OrganizationId,
    string UserId,
    string ExternalTransactionId,
    decimal Amount,
    string Currency,
    string MerchantName,
    string Reason,
    string Status,
    DateTimeOffset ExpiresAtUtc,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? DecidedAtUtc,
    string? DecisionNote);
