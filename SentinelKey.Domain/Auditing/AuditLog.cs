using SentinelKey.Domain.Common;

namespace SentinelKey.Domain.Auditing;

public sealed class AuditLog : Entity
{
    private AuditLog()
    {
    }

    public AuditLog(
        string actionType,
        string actorId,
        string actorType,
        string targetId,
        string targetType,
        string description,
        string correlationId)
    {
        ActionType = actionType;
        ActorId = actorId;
        ActorType = actorType;
        TargetId = targetId;
        TargetType = targetType;
        Description = description;
        CorrelationId = correlationId;
    }

    public string ActionType { get; private set; } = string.Empty;
    public string ActorId { get; private set; } = string.Empty;
    public string ActorType { get; private set; } = string.Empty;
    public string TargetId { get; private set; } = string.Empty;
    public string TargetType { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string CorrelationId { get; private set; } = string.Empty;
}
