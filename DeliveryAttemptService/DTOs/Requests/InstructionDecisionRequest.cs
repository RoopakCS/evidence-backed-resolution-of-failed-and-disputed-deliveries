using System.ComponentModel.DataAnnotations;
using DeliveryAttemptService.Models.Enums;

namespace DeliveryAttemptService.DTOs.Requests;

public class InstructionDecisionRequest
{
    [Required(ErrorMessage = "InstructionId is required.")]
    public Guid InstructionId { get; set; }

    [Required(ErrorMessage = "ShipmentId is required.")]
    public Guid ShipmentId { get; set; }

    [Required(ErrorMessage = "Decision is required.")]
    public DriverInstructionDecision Decision { get; set; }

    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
    public string? Reason { get; set; }

    [Required(ErrorMessage = "DecidedAt is required.")]
    public DateTime DecidedAt { get; set; }

    [Required(ErrorMessage = "IdempotencyKey is required.")]
    [StringLength(100, ErrorMessage = "IdempotencyKey cannot exceed 100 characters.")]
    public string IdempotencyKey { get; set; } = string.Empty;
}
