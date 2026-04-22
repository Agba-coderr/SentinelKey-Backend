using SentinelKey.Domain.Devices;

namespace SentinelKey.Application.Common.Abstractions.Repositories;

public interface IDeviceRepository
{
    Task AddAsync(Device device, CancellationToken cancellationToken);
    Task<Device?> GetByIdAsync(Guid deviceId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Device>> ListByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken);
    Task UpdateAsync(Device device, CancellationToken cancellationToken);
}
