namespace DeliveryAttemptService.DTOs.Common;

public class DriverInstructionSummaryDto
{
    public Guid Id { get; set; }

    public string InstructionType { get; set; } = string.Empty;

    public string InstructionText { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}
