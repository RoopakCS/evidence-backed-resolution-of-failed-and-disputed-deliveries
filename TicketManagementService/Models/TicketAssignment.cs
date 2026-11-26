namespace TicketManagementService.Models;

/// <summary>
/// Tracks the full assignment history for a ticket.
/// Use when assignment audit trail matters; otherwise AssignedAgentId on Ticket is sufficient.
/// </summary>
public class TicketAssignment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Local FK to the parent ticket.
    /// </summary>
    public Guid TicketId { get; set; }

    /// <summary>
    /// Agent to whom the ticket was assigned (plain UUID reference to Identity Service).
    /// </summary>
    public Guid AssignedToAgentId { get; set; }

    /// <summary>
    /// User who performed the assignment (plain UUID reference to Identity Service).
    /// </summary>
    public Guid AssignedByUserId { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UnassignedAt { get; set; }

    public string? Reason { get; set; }

    // Navigation property
    public Ticket? Ticket { get; set; }
}
