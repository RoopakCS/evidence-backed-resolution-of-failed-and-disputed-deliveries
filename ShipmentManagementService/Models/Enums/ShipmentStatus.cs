namespace ShipmentManagementService.Models.Enums;

public enum ShipmentStatus
{
    Created,
    Pending,
    InTransit,
    OutForDelivery,
    Delivered,
    DeliveryFailed,
    Rescheduled,
    ReturnInitiated,
    Returned,
    Cancelled
}
