using ShipmentManagementService.Models.Enums;

namespace ShipmentManagementService.DTOs.Responses;

public class RescheduleResponse
{
    public Guid RequestId { get; set; }

    public Guid ShipmentId { get; set; }

    public DateTime RequestedDeliveryDate { get; set; }

    public string Reason { get; set; } = string.Empty;

    public RescheduleStatus Status { get; set; }

    public DateTime RequestedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public Guid? ProcessedByUserId { get; set; }
}
