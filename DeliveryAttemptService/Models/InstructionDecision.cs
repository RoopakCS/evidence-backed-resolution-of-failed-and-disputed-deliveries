using DeliveryAttemptService.Models.Enums;

namespace DeliveryAttemptService.Models;

/// <summary>
/// Immutable record of driver's acceptance or decline decision on a customer delivery instruction.
/// InstructionId belongs to Shipment Service (external plain UUID).
/// </summary>
public class InstructionDecision
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// External identifier of the instruction from Shipment Service.
    /// </summary>
    public Guid InstructionId { get; set; }

    /// <summary>
    /// External identifier of the shipment from Shipment Service.
    /// </summary>
    public Guid ShipmentId { get; set; }

    /// <summary>
    /// Driver identifier from Identity Service.
    /// </summary>
    public Guid DriverId { get; set; }

    public DriverInstructionDecision Decision { get; set; }

    public string? Reason { get; set; }

    public DateTime DecidedAt { get; set; }

    /// <summary>
    /// Unique idempotency key to prevent duplicate decision records.
    /// </summary>
    public string IdempotencyKey { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Optional navigation to parent attempt if captured together
    public Guid? DeliveryAttemptId { get; set; }
    public DeliveryAttempt? DeliveryAttempt { get; set; }
}
