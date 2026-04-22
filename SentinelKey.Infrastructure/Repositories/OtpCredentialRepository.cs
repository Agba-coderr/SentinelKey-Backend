using Microsoft.EntityFrameworkCore;
using SentinelKey.Application.Common.Abstractions.Repositories;
using SentinelKey.Domain.Otp;
using SentinelKey.Infrastructure.Persistence;

namespace SentinelKey.Infrastructure.Repositories;

public sealed class OtpCredentialRepository : IOtpCredentialRepository
{
    private readonly SentinelKeyDbContext _dbContext;

    public OtpCredentialRepository(SentinelKeyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(OtpCredential otpCredential, CancellationToken cancellationToken)
    {
        await _dbContext.OtpCredentialSet.AddAsync(otpCredential, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<OtpCredential?> GetByDeviceAndUserAsync(Guid deviceId, string userId, CancellationToken cancellationToken)
    {
        var normalizedUserId = userId.Trim();
        return _dbContext.OtpCredentialSet.FirstOrDefaultAsync(
            x => x.DeviceId == deviceId && x.UserId == normalizedUserId,
            cancellationToken);
    }
}
