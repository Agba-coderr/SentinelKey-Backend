using Microsoft.Extensions.DependencyInjection;
using SentinelKey.Application.Devices.DeviceManagement;
using SentinelKey.Application.Enrollment;
using SentinelKey.Application.Enrollment.CreateEnrollmentSession;
using SentinelKey.Application.Otp.ValidateOtp;
using SentinelKey.Application.Organizations.CreateOrganization;
using SentinelKey.Application.Transactions;

namespace SentinelKey.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IOrganizationOnboardingService, OrganizationOnboardingService>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();
        services.AddScoped<IDeviceManagementService, DeviceManagementService>();
        services.AddScoped<IOtpValidationService, OtpValidationService>();
        services.AddScoped<ITransactionChallengeService, TransactionChallengeService>();
        return services;
    }
}
