namespace SentinelKey.Contracts.Transactions;

public sealed record CreateTransactionChallengeRequest(
    string UserId,
    string ExternalTransactionId,
    decimal Amount,
    string Currency,
    string MerchantName,
    string Reason,
    int ExpiresInMinutes = 5);
