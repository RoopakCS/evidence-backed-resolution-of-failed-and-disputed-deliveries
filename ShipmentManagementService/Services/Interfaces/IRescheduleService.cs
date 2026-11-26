using ShipmentManagementService.DTOs.Requests;
using ShipmentManagementService.DTOs.Responses;

namespace ShipmentManagementService.Services.Interfaces;

public interface IRescheduleService
{
    Task<List<RescheduleResponse>> ListByShipmentAsync(Guid shipmentId, Guid actorUserId, string actorRole, CancellationToken ct = default);
    Task<RescheduleResponse> CreateAsync(Guid shipmentId, RescheduleRequestDto request, Guid actorUserId, string actorRole, string? idempotencyKey = null, CancellationToken ct = default);
    Task<RescheduleResponse> ProcessAsync(Guid shipmentId, Guid requestId, ProcessRescheduleRequest request, Guid actorUserId, string? idempotencyKey = null, CancellationToken ct = default);
}
