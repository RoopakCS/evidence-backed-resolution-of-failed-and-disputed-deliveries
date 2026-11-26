using System.ComponentModel.DataAnnotations;
using TicketManagementService.Models.Enums;

namespace TicketManagementService.DTOs.Requests;

public class CreateTicketRequest
{
    [Required(ErrorMessage = "ShipmentId is required.")]
    public Guid ShipmentId { get; set; }

    public Guid? DeliveryAttemptId { get; set; }

    [Required(ErrorMessage = "TicketType is required.")]
    public TicketType TicketType { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(4000, ErrorMessage = "Description cannot exceed 4000 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Source is required.")]
    public TicketSource Source { get; set; }

    /// <summary>
    /// Summary of the AI assistant conversation (used for assistant handover tickets).
    /// </summary>
    [StringLength(8000, ErrorMessage = "Conversation summary cannot exceed 8000 characters.")]
    public string? ConversationSummary { get; set; }

    /// <summary>
    /// Optional references to evidence items supporting this ticket.
    /// </summary>
    public List<Guid>? EvidenceReferences { get; set; }
}
