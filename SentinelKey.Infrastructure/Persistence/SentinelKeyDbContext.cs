using Microsoft.EntityFrameworkCore;
using SentinelKey.Application.Common.Abstractions;
using SentinelKey.Domain.Auditing;
using SentinelKey.Domain.Devices;
using SentinelKey.Domain.Enrollment;
using SentinelKey.Domain.Organizations;
using SentinelKey.Domain.Otp;
using SentinelKey.Domain.Transactions;

namespace SentinelKey.Infrastructure.Persistence;

public sealed class SentinelKeyDbContext : DbContext, IApplicationDbContext
{
    public SentinelKeyDbContext(DbContextOptions<SentinelKeyDbContext> options)
        : base(options)
    {
    }

    public DbSet<Organization> OrganizationSet => Set<Organization>();
    public DbSet<PlatformConfiguration> PlatformConfigurationSet => Set<PlatformConfiguration>();
    public DbSet<AuditLog> AuditLogSet => Set<AuditLog>();
    public DbSet<EnrollmentSession> EnrollmentSessionSet => Set<EnrollmentSession>();
    public DbSet<Device> DeviceSet => Set<Device>();
    public DbSet<OtpCredential> OtpCredentialSet => Set<OtpCredential>();
    public DbSet<TransactionChallenge> TransactionChallengeSet => Set<TransactionChallenge>();

    public IQueryable<Organization> Organizations => OrganizationSet.AsQueryable();
    public IQueryable<AuditLog> AuditLogs => AuditLogSet.AsQueryable();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SentinelKeyDbContext).Assembly);
    }
}
