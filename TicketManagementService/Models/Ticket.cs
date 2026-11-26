using TicketManagementService.Models.Enums;

namespace TicketManagementService.Models;

/// <summary>
/// Core ticket entity representing an exception or dispute case.
/// External service IDs (ShipmentId, CustomerId, DriverId) are plain UUIDs.
/// DeliveryAttemptId may be absent for customer-created disputes without an attempt reference.
/// </summary>
public class Ticket
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Human-readable unique ticket number (e.g. TKT-20261126-0001).
    /// </summary>
    public string TicketNumber { get; set; } = string.Empty;

    /// <summary>
    /// Shipment identifier (plain UUID reference to Shipment Service).
    /// </summary>
    public Guid ShipmentId { get; set; }

    /// <summary>
    /// Delivery attempt identifier (plain UUID reference to Delivery Attempt Service).
    /// Nullable for customer-created disputes without an attempt reference.
    /// </summary>
    public Guid? DeliveryAttemptId { get; set; }

    /// <summary>
    /// Customer identifier (plain UUID reference to Identity Service).
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Driver identifier (plain UUID reference to Identity Service).
    /// Nullable when the ticket is not associated with a specific driver.
    /// </summary>
    public Guid? DriverId { get; set; }

    public TicketSource Source { get; set; }

    public TicketType TicketType { get; set; }

    public TicketSeverity Severity { get; set; }

    public TicketStatus Status { get; set; } = TicketStatus.Open;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Reference to the ExceptionRule that created this ticket (nullable).
    /// Only set when Source is AutomaticRule.
    /// </summary>
    public Guid? MatchingRuleId { get; set; }

    /// <summary>
    /// Currently assigned operations agent (plain UUID reference to Identity Service).
    /// </summary>
    public Guid? AssignedAgentId { get; set; }

    /// <summary>
    /// Resolution description. Required when status transitions to Resolved.
    /// </summary>
    public string? Resolution { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ResolvedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    // Navigation collections
    public ICollection<TicketEvent> Events { get; set; } = new List<TicketEvent>();

    public ICollection<TicketAssignment> Assignments { get; set; } = new List<TicketAssignment>();
}
