using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SentinelKey.Application.Common.Abstractions.Repositories;
using SentinelKey.Application.Common.Abstractions.Security;
using SentinelKey.Infrastructure.Persistence;
using SentinelKey.Infrastructure.Repositories;
using SentinelKey.Infrastructure.Security;

namespace SentinelKey.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SentinelKey");
        services.AddMemoryCache();

        services.AddDbContext<SentinelKeyDbContext>(options =>
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseInMemoryDatabase("sentinelkey-dev");
                return;
            }

            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<IOtpCredentialRepository, OtpCredentialRepository>();
        services.AddScoped<ITransactionChallengeRepository, TransactionChallengeRepository>();
        services.AddSingleton<IOtpSecretProtector, OtpSecretProtector>();
        services.AddSingleton<IReplayProtectionService, InMemoryReplayProtectionService>();
        services.AddSingleton<IRateLimitService, InMemoryRateLimitService>();

        return services;
    }
}
