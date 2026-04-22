namespace SentinelKey.Application.Transactions.DecideTransactionChallenge;

public sealed record DecideTransactionChallengeCommand(
    Guid ChallengeId,
    bool Approve,
    string? DecisionNote,
    string ActorId,
    string CorrelationId);
