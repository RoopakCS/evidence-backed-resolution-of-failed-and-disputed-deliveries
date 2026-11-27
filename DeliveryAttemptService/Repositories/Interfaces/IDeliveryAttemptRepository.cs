using DeliveryAttemptService.Models;

namespace DeliveryAttemptService.Repositories.Interfaces;

public interface IDeliveryAttemptRepository
{
    Task<DeliveryAttempt?> GetAttemptByIdAsync(Guid attemptId, bool includeEvidence = true, CancellationToken ct = default);

    Task<List<DeliveryAttempt>> GetAttemptsByShipmentAsync(Guid shipmentId, CancellationToken ct = default);

    Task AddAttemptAsync(DeliveryAttempt attempt, CancellationToken ct = default);

    Task AddInstructionDecisionAsync(InstructionDecision decision, CancellationToken ct = default);

    Task<List<InstructionDecision>> GetDecisionsByShipmentAsync(Guid shipmentId, CancellationToken ct = default);

    Task<Evidence?> GetEvidenceByIdAsync(Guid evidenceId, CancellationToken ct = default);

    Task<List<Evidence>> GetEvidenceByAttemptIdAsync(Guid attemptId, CancellationToken ct = default);

    Task AddEvidenceAsync(Evidence evidence, CancellationToken ct = default);

    Task AddOutboxMessageAsync(OutboxMessage message, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}
