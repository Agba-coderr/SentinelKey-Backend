using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SentinelKey.Domain.Organizations;

namespace SentinelKey.Infrastructure.Persistence.Configurations;

public sealed class PlatformConfigurationConfiguration : IEntityTypeConfiguration<PlatformConfiguration>
{
    public void Configure(EntityTypeBuilder<PlatformConfiguration> builder)
    {
        builder.ToTable("platform_configurations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PlatformName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.CallbackBaseUrl)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.PushChallengesEnabled)
            .IsRequired();

        builder.Property(x => x.TransactionChallengesEnabled)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();
    }
}
