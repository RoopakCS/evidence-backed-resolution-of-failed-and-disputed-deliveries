using System.ComponentModel.DataAnnotations;

namespace TicketManagementService.DTOs.Requests;

public class AssignTicketRequest
{
    [Required(ErrorMessage = "AgentId is required.")]
    public Guid AgentId { get; set; }

    [StringLength(1000, ErrorMessage = "Reason cannot exceed 1000 characters.")]
    public string? Reason { get; set; }
}
