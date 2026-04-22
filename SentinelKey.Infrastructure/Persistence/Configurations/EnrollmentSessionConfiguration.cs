using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SentinelKey.Domain.Enrollment;

namespace SentinelKey.Infrastructure.Persistence.Configurations;

public sealed class EnrollmentSessionConfiguration : IEntityTypeConfiguration<EnrollmentSession>
{
    public void Configure(EntityTypeBuilder<EnrollmentSession> builder)
    {
        builder.ToTable("enrollment_sessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.QrCodePayload)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.ExpiresAtUtc)
            .IsRequired();
    }
}
