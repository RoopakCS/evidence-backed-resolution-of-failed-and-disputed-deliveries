using TicketManagementService.Models.Enums;

namespace TicketManagementService.DTOs.Responses;

/// <summary>
/// Lightweight projection for dashboard list views.
/// </summary>
public class TicketListItemResponse
{
    public string TicketNumber { get; set; } = string.Empty;

    public Guid ShipmentId { get; set; }

    public TicketType TicketType { get; set; }

    public TicketSeverity Severity { get; set; }

    public TicketStatus Status { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public Guid? AssignedAgentId { get; set; }
}
