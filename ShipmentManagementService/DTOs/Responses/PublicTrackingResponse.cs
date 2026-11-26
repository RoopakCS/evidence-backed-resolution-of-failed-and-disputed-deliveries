using ShipmentManagementService.Models.Enums;

namespace ShipmentManagementService.DTOs.Responses;

/// <summary>
/// Publicly accessible tracking summary without customer PII.
/// </summary>
public class PublicTrackingResponse
{
    public string TrackingNumber { get; set; } = string.Empty;

    public ShipmentStatus Status { get; set; }

    public DateTime StatusUpdatedAt { get; set; }

    public TimeSpan StatusAge { get; set; }

    public DateTime ExpectedDeliveryDate { get; set; }

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public List<ShipmentStatusHistoryResponse> History { get; set; } = new();
}
