using TicketManagementService.Models.Enums;

namespace TicketManagementService.DTOs.Responses;

public class TicketEventResponse
{
    public Guid EventId { get; set; }

    public TicketEventType EventType { get; set; }

    public string Description { get; set; } = string.Empty;

    public Guid? ActorId { get; set; }

    public string ActorRole { get; set; } = string.Empty;

    public string? Metadata { get; set; }

    public DateTime CreatedAt { get; set; }
}
