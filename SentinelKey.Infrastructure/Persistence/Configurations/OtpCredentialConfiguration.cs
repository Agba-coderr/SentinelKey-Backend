using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SentinelKey.Domain.Otp;

namespace SentinelKey.Infrastructure.Persistence.Configurations;

public sealed class OtpCredentialConfiguration : IEntityTypeConfiguration<OtpCredential>
{
    public void Configure(EntityTypeBuilder<OtpCredential> builder)
    {
        builder.ToTable("otp_credentials");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.EncryptedSecret)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Algorithm)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Digits)
            .IsRequired();

        builder.Property(x => x.PeriodSeconds)
            .IsRequired();
    }
}
