using ShipmentManagementService.Models.Enums;

namespace ShipmentManagementService.DTOs.Responses;

public class InstructionResponse
{
    public Guid Id { get; set; }

    public Guid ShipmentId { get; set; }

    public InstructionType Type { get; set; }

    public string Text { get; set; } = string.Empty;

    public InstructionStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
