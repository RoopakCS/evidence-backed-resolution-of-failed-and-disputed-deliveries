using ShipmentManagementService.DTOs.Requests;
using ShipmentManagementService.DTOs.Responses;

namespace ShipmentManagementService.Services.Interfaces;

public interface IInstructionService
{
    Task<List<InstructionResponse>> ListByShipmentAsync(Guid shipmentId, Guid actorUserId, string actorRole, CancellationToken ct = default);
    Task<InstructionResponse> CreateAsync(Guid shipmentId, CreateInstructionRequest request, Guid actorUserId, string actorRole, string? idempotencyKey = null, CancellationToken ct = default);
    Task<InstructionResponse> UpdateAsync(Guid shipmentId, Guid instructionId, UpdateInstructionRequest request, Guid actorUserId, string actorRole, string? idempotencyKey = null, CancellationToken ct = default);
}
