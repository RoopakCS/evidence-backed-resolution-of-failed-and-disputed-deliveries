using ShipmentManagementService.DTOs.Requests;
using ShipmentManagementService.DTOs.Responses;

namespace ShipmentManagementService.Services.Interfaces;

public interface IShipmentService
{
    Task<ShipmentResponse> CreateAsync(CreateShipmentRequest request, Guid actorUserId, string actorRole, CancellationToken ct = default);
    Task<ShipmentResponse> GetByIdAsync(Guid id, Guid actorUserId, string actorRole, CancellationToken ct = default);
    Task<List<ShipmentSummaryResponse>> ListAsync(Guid actorUserId, string actorRole, int page, int pageSize, CancellationToken ct = default);
    Task<PublicTrackingResponse> TrackByNumberAsync(string trackingNumber, CancellationToken ct = default);
    Task<ShipmentResponse> UpdateAsync(Guid id, UpdateShipmentRequest request, Guid actorUserId, string actorRole, CancellationToken ct = default);
    Task<ShipmentStatusHistoryResponse> RecordTrackingUpdateAsync(Guid id, RecordTrackingUpdateRequest request, Guid actorUserId, CancellationToken ct = default);
    Task<List<ShipmentStatusHistoryResponse>> GetStatusHistoryAsync(Guid id, Guid actorUserId, string actorRole, CancellationToken ct = default);
}
