using TicketManagementService.Models.Enums;

namespace TicketManagementService.DTOs.Responses;

public class TicketResponse
{
    public Guid TicketId { get; set; }

    public string TicketNumber { get; set; } = string.Empty;

    public Guid ShipmentId { get; set; }

    public Guid? DeliveryAttemptId { get; set; }

    public Guid CustomerId { get; set; }

    public Guid? DriverId { get; set; }

    public TicketSource Source { get; set; }

    public TicketType TicketType { get; set; }

    public TicketSeverity Severity { get; set; }

    public TicketStatus Status { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Guid? MatchingRuleId { get; set; }

    public Guid? AssignedAgentId { get; set; }

    public string? Resolution { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public DateTime? ClosedAt { get; set; }
}
