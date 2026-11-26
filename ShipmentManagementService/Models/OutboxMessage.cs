namespace ShipmentManagementService.Models;

/// <summary>
/// Transactional outbox entry for domain events to be dispatched by a message broker worker.
/// </summary>
public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string EventType { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;

    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

    public DateTime? ProcessedAt { get; set; }
}
