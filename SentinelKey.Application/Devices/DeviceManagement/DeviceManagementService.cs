using SentinelKey.Application.Common.Abstractions.Repositories;
using SentinelKey.Contracts.Devices;
using SentinelKey.Domain.Auditing;
using SentinelKey.Domain.Devices;

namespace SentinelKey.Application.Devices.DeviceManagement;

public sealed class DeviceManagementService : IDeviceManagementService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public DeviceManagementService(
        IDeviceRepository deviceRepository,
        IAuditLogRepository auditLogRepository)
    {
        _deviceRepository = deviceRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<IReadOnlyCollection<DeviceResponse>> ListByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        var devices = await _deviceRepository.ListByOrganizationAsync(organizationId, cancellationToken);
        return devices.Select(ToResponse).ToArray();
    }

    public async Task<DeviceResponse> SuspendAsync(
        Guid deviceId,
        string actorId,
        string correlationId,
        CancellationToken cancellationToken)
    {
        var device = await _deviceRepository.GetByIdAsync(deviceId, cancellationToken);
        if (device is null)
        {
            throw new InvalidOperationException("Device was not found.");
        }

        device.Suspend();
        await _deviceRepository.UpdateAsync(device, cancellationToken);

        await _auditLogRepository.AddAsync(
            new AuditLog(
                AuditActionType.DeviceSuspended,
                actorId,
                "api-client",
                device.Id.ToString(),
                nameof(Device),
                $"Device '{device.DeviceName}' was suspended.",
                correlationId),
            cancellationToken);

        return ToResponse(device);
    }

    private static DeviceResponse ToResponse(Device device)
    {
        return new DeviceResponse(
            device.Id,
            device.OrganizationId,
            device.UserId,
            device.DeviceIdentifier,
            device.DeviceName,
            device.Platform,
            device.AppVersion,
            device.Status.ToString(),
            device.CreatedAtUtc,
            device.BoundAtUtc,
            device.SuspendedAtUtc);
    }
}
