using ShipmentManagementService.DTOs.Requests;
using ShipmentManagementService.DTOs.Responses;
using ShipmentManagementService.Models.Enums;

namespace ShipmentManagementService.Services.Interfaces;

public interface IReturnService
{
    Task<List<ReturnResponse>> ListByShipmentAsync(Guid shipmentId, Guid actorUserId, string actorRole, CancellationToken ct = default);
    Task<ReturnResponse> InitiateAsync(Guid shipmentId, InitiateReturnRequest request, Guid actorUserId, string actorRole, string? idempotencyKey = null, CancellationToken ct = default);
    Task<ReturnResponse> UpdateStatusAsync(Guid shipmentId, Guid returnId, ReturnStatus newStatus, Guid actorUserId, CancellationToken ct = default);
}
