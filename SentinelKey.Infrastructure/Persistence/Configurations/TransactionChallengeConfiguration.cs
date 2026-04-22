using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SentinelKey.Domain.Transactions;

namespace SentinelKey.Infrastructure.Persistence.Configurations;

public sealed class TransactionChallengeConfiguration : IEntityTypeConfiguration<TransactionChallenge>
{
    public void Configure(EntityTypeBuilder<TransactionChallenge> builder)
    {
        builder.ToTable("transaction_challenges");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ExternalTransactionId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Currency)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.MerchantName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Reason)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.DecisionNote)
            .HasMaxLength(300);

        builder.HasIndex(x => x.ExternalTransactionId);
    }
}
