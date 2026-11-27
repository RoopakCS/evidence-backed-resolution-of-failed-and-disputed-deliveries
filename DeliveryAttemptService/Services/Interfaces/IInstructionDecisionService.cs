using DeliveryAttemptService.DTOs.Requests;
using DeliveryAttemptService.DTOs.Responses;

namespace DeliveryAttemptService.Services.Interfaces;

public interface IInstructionDecisionService
{
    Task<(InstructionDecisionResponse Response, bool AlreadyProcessed)> RecordDecisionAsync(
        Guid driverId,
        InstructionDecisionRequest request,
        CancellationToken ct = default);

    Task<List<InstructionDecisionResponse>> GetDecisionsByShipmentAsync(
        Guid shipmentId,
        Guid callerId,
        string callerRole,
        CancellationToken ct = default);
}
