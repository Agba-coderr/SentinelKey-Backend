namespace SentinelKey.Application.Transactions.CreateTransactionChallenge;

public sealed record CreateTransactionChallengeCommand(
    Guid OrganizationId,
    string UserId,
    string ExternalTransactionId,
    decimal Amount,
    string Currency,
    string MerchantName,
    string Reason,
    int ExpiresInMinutes,
    string ActorId,
    string CorrelationId);
