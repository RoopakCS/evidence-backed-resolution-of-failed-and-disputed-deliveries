using ShipmentManagementService.Models.Enums;

namespace ShipmentManagementService.DTOs.Responses;

public class ReturnResponse
{
    public Guid ReturnId { get; set; }

    public Guid ShipmentId { get; set; }

    public string ReturnReason { get; set; } = string.Empty;

    public ReturnStatus ReturnStatus { get; set; }

    public DateTime InitiatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
