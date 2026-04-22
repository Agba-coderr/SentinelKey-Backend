using SentinelKey.Contracts.Devices;

namespace SentinelKey.Application.Devices.DeviceManagement;

public interface IDeviceManagementService
{
    Task<IReadOnlyCollection<DeviceResponse>> ListByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken);
    Task<DeviceResponse> SuspendAsync(Guid deviceId, string actorId, string correlationId, CancellationToken cancellationToken);
}
