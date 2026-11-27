using DeliveryAttemptService.DTOs.Requests;
using DeliveryAttemptService.DTOs.Responses;

namespace DeliveryAttemptService.Services.Interfaces;

public interface IOfflineSyncService
{
    Task<SyncOfflineActionsResponse> SyncAsync(
        Guid driverId,
        SyncOfflineActionsRequest request,
        CancellationToken ct = default);
}
