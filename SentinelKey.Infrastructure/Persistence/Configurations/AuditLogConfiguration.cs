using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SentinelKey.Domain.Auditing;

namespace SentinelKey.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ActionType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ActorId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ActorType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.TargetId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.TargetType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.CorrelationId)
            .HasMaxLength(100)
            .IsRequired();
    }
}
