using DeliveryAttemptService.Models.Enums;

namespace DeliveryAttemptService.DTOs.Responses;

public class InstructionDecisionResponse
{
    public Guid DecisionId { get; set; }

    public Guid InstructionId { get; set; }

    public Guid ShipmentId { get; set; }

    public Guid DriverId { get; set; }

    public DriverInstructionDecision Decision { get; set; }

    public string? Reason { get; set; }

    public DateTime DecidedAt { get; set; }

    public DateTime CreatedAt { get; set; }
}
