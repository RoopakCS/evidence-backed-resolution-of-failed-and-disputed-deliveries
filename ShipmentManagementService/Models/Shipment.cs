using ShipmentManagementService.Models.Enums;

namespace ShipmentManagementService.Models;

public class Shipment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string TrackingNumber { get; set; } = string.Empty;

    /// <summary>
    /// Identity ID of the customer who owns or ordered this shipment.
    /// Not a cross-database foreign key.
    /// </summary>
    public Guid CustomerId { get; set; }

    public string RecipientName { get; set; } = string.Empty;

    public string RecipientPhone { get; set; } = string.Empty;

    public string AddressLine1 { get; set; } = string.Empty;

    public string? AddressLine2 { get; set; }

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public ShipmentStatus Status { get; set; } = ShipmentStatus.Created;

    public DateTime ExpectedDeliveryDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation collections
    public ICollection<DeliveryInstruction> Instructions { get; set; } = new List<DeliveryInstruction>();

    public ICollection<RescheduleRequest> RescheduleRequests { get; set; } = new List<RescheduleRequest>();

    public ICollection<Return> Returns { get; set; } = new List<Return>();

    public ICollection<ShipmentStatusHistory> StatusHistory { get; set; } = new List<ShipmentStatusHistory>();
}
