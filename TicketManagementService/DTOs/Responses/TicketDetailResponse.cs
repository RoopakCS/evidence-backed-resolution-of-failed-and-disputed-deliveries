namespace TicketManagementService.DTOs.Responses;

/// <summary>
/// Full ticket detail including timeline events and linked evidence/attempt references.
/// Evidence details are fetched from Delivery Attempt Service.
/// </summary>
public class TicketDetailResponse : TicketResponse
{
    /// <summary>
    /// Ordered timeline of ticket events.
    /// </summary>
    public List<TicketEventResponse> Events { get; set; } = new();

    /// <summary>
    /// Evidence references linked to this ticket (fetched from Delivery Attempt Service).
    /// </summary>
    public List<Guid> LinkedEvidenceIds { get; set; } = new();

    /// <summary>
    /// Delivery attempt references linked to this ticket.
    /// </summary>
    public List<Guid> LinkedAttemptIds { get; set; } = new();
}
