using System.ComponentModel.DataAnnotations;
using DeliveryAttemptService.DTOs.Common;

namespace DeliveryAttemptService.DTOs.Requests;

public class CreateDriverAssignmentRequest : IValidatableObject
{
    [Required]
    public Guid ShipmentId { get; set; }

    [Required]
    public Guid DriverId { get; set; }

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    [StringLength(64)]
    public string TrackingNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string RecipientName { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string RecipientPhone { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string AddressLine1 { get; set; } = string.Empty;

    [StringLength(200)]
    public string? AddressLine2 { get; set; }

    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string State { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string PostalCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Country { get; set; } = string.Empty;

    [Range(-90.0, 90.0)]
    public double? Latitude { get; set; }

    [Range(-180.0, 180.0)]
    public double? Longitude { get; set; }

    public DateTime? ScheduledDate { get; set; }

    public List<DriverInstructionSummaryDto> Instructions { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ShipmentId == Guid.Empty)
            yield return new ValidationResult("ShipmentId must not be empty.", new[] { nameof(ShipmentId) });
        if (DriverId == Guid.Empty)
            yield return new ValidationResult("DriverId must not be empty.", new[] { nameof(DriverId) });
        if (CustomerId == Guid.Empty)
            yield return new ValidationResult("CustomerId must not be empty.", new[] { nameof(CustomerId) });
    }
}
