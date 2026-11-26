using TicketManagementService.Models.Enums;

namespace TicketManagementService.Models;

/// <summary>
/// Append-only timeline/audit record for a ticket.
/// Each state change, assignment, or comment creates a new event.
/// </summary>
public class TicketEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Local FK to the parent ticket.
    /// </summary>
    public Guid TicketId { get; set; }

    public TicketEventType EventType { get; set; }

    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The user/system that performed the action (nullable for system events).
    /// </summary>
    public Guid? ActorId { get; set; }

    /// <summary>
    /// Role of the actor (e.g. "Customer", "Agent", "System", "Driver").
    /// </summary>
    public string ActorRole { get; set; } = string.Empty;

    /// <summary>
    /// Optional JSON metadata for event-specific data.
    /// </summary>
    public string? Metadata { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Ticket? Ticket { get; set; }
}
