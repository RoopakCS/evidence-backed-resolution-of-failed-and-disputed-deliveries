using System.ComponentModel.DataAnnotations;

namespace TicketManagementService.DTOs.Requests;

/// <summary>
/// Specialised request for tickets created via AI assistant escalation.
/// Source is always AssistantEscalation.
/// </summary>
public class CreateTicketFromAssistantRequest
{
    [Required(ErrorMessage = "ShipmentId is required.")]
    public Guid ShipmentId { get; set; }

    public Guid? DeliveryAttemptId { get; set; }

    [Required(ErrorMessage = "Conversation summary is required for assistant escalations.")]
    [StringLength(8000, ErrorMessage = "Conversation summary cannot exceed 8000 characters.")]
    public string ConversationSummary { get; set; } = string.Empty;

    [Required(ErrorMessage = "Customer issue description is required.")]
    [StringLength(4000, ErrorMessage = "Customer issue cannot exceed 4000 characters.")]
    public string CustomerIssue { get; set; } = string.Empty;
}
