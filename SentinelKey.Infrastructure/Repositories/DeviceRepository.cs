using Microsoft.EntityFrameworkCore;
using SentinelKey.Application.Common.Abstractions.Repositories;
using SentinelKey.Domain.Devices;
using SentinelKey.Infrastructure.Persistence;

namespace SentinelKey.Infrastructure.Repositories;

public sealed class DeviceRepository : IDeviceRepository
{
    private readonly SentinelKeyDbContext _dbContext;

    public DeviceRepository(SentinelKeyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Device device, CancellationToken cancellationToken)
    {
        await _dbContext.DeviceSet.AddAsync(device, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Device?> GetByIdAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        return _dbContext.DeviceSet.FirstOrDefaultAsync(x => x.Id == deviceId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Device>> ListByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        return await _dbContext.DeviceSet
            .Where(x => x.OrganizationId == organizationId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);
    }

    public async Task UpdateAsync(Device device, CancellationToken cancellationToken)
    {
        _dbContext.DeviceSet.Update(device);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
