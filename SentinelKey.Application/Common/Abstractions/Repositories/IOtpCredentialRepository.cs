using SentinelKey.Domain.Otp;

namespace SentinelKey.Application.Common.Abstractions.Repositories;

public interface IOtpCredentialRepository
{
    Task AddAsync(OtpCredential otpCredential, CancellationToken cancellationToken);
    Task<OtpCredential?> GetByDeviceAndUserAsync(Guid deviceId, string userId, CancellationToken cancellationToken);
}
