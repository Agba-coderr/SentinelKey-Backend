using SentinelKey.Domain.Common;

namespace SentinelKey.Domain.Transactions;

public sealed class TransactionChallenge : Entity
{
    private TransactionChallenge()
    {
    }

    public TransactionChallenge(
        Guid organizationId,
        string userId,
        string externalTransactionId,
        decimal amount,
        string currency,
        string merchantName,
        string reason,
        DateTimeOffset expiresAtUtc)
    {
        OrganizationId = organizationId;
        UserId = userId.Trim();
        ExternalTransactionId = externalTransactionId.Trim();
        Amount = amount;
        Currency = currency.Trim().ToUpperInvariant();
        MerchantName = merchantName.Trim();
        Reason = reason.Trim();
        ExpiresAtUtc = expiresAtUtc;
        Status = TransactionChallengeStatus.Pending;
    }

    public Guid OrganizationId { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string ExternalTransactionId { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public string MerchantName { get; private set; } = string.Empty;
    public string Reason { get; private set; } = string.Empty;
    public TransactionChallengeStatus Status { get; private set; }
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public DateTimeOffset? DecidedAtUtc { get; private set; }
    public string? DecisionNote { get; private set; }

    public void Approve(string? decisionNote)
    {
        EnsurePending();
        Status = TransactionChallengeStatus.Approved;
        DecidedAtUtc = DateTimeOffset.UtcNow;
        DecisionNote = decisionNote?.Trim();
        Touch();
    }

    public void Reject(string? decisionNote)
    {
        EnsurePending();
        Status = TransactionChallengeStatus.Rejected;
        DecidedAtUtc = DateTimeOffset.UtcNow;
        DecisionNote = decisionNote?.Trim();
        Touch();
    }

    private void EnsurePending()
    {
        if (Status != TransactionChallengeStatus.Pending)
        {
            throw new InvalidOperationException("Only pending transaction challenges can be decided.");
        }

        if (ExpiresAtUtc <= DateTimeOffset.UtcNow)
        {
            throw new InvalidOperationException("This transaction challenge has expired.");
        }
    }
}
