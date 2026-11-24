using DeliveryAttemptService.Models.Enums;

namespace DeliveryAttemptService.Models;

/// <summary>
/// Append-only record of a completed delivery attempt.
/// CapturedAt is client/device time; ReceivedAt is server ingestion time.
/// </summary>
public class DeliveryAttempt
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Shipment identifier (plain UUID reference to Shipment Service).
    /// </summary>
    public Guid ShipmentId { get; set; }

    /// <summary>
    /// Driver identifier (plain UUID reference to Identity Service).
    /// </summary>
    public Guid DriverId { get; set; }

    public AttemptResult Result { get; set; }

    public FailureReason? FailureReason { get; set; }

    public string? AttemptNote { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    /// <summary>
    /// Timestamp when attempt was captured on driver's device (UTC).
    /// </summary>
    public DateTime CapturedAt { get; set; }

    /// <summary>
    /// Timestamp when attempt was received by the server (UTC).
    /// </summary>
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Unique idempotency key to prevent duplicate delivery attempt records.
    /// </summary>
    public string IdempotencyKey { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation collections
    public ICollection<Evidence> EvidenceItems { get; set; } = new List<Evidence>();

    public ICollection<InstructionDecision> InstructionDecisions { get; set; } = new List<InstructionDecision>();
}
