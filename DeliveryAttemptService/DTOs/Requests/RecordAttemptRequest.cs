using System.ComponentModel.DataAnnotations;
using DeliveryAttemptService.Models.Enums;

namespace DeliveryAttemptService.DTOs.Requests;

public class RecordAttemptRequest : IValidatableObject
{
    [Required(ErrorMessage = "ShipmentId is required.")]
    public Guid ShipmentId { get; set; }

    [Required(ErrorMessage = "Result is required.")]
    public AttemptResult Result { get; set; }

    public FailureReason? FailureReason { get; set; }

    [StringLength(1000, ErrorMessage = "Attempt note cannot exceed 1000 characters.")]
    public string? AttemptNote { get; set; }

    [Range(-90.0, 90.0, ErrorMessage = "Latitude must be between -90 and 90 degrees.")]
    public double? Latitude { get; set; }

    [Range(-180.0, 180.0, ErrorMessage = "Longitude must be between -180 and 180 degrees.")]
    public double? Longitude { get; set; }

    [Required(ErrorMessage = "CapturedAt is required.")]
    public DateTime CapturedAt { get; set; }

    [Required(ErrorMessage = "IdempotencyKey is required.")]
    [StringLength(100, ErrorMessage = "IdempotencyKey cannot exceed 100 characters.")]
    public string IdempotencyKey { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ShipmentId == Guid.Empty)
            yield return new ValidationResult("ShipmentId must not be empty.", new[] { nameof(ShipmentId) });
        if (CapturedAt == default)
            yield return new ValidationResult("CapturedAt must be provided.", new[] { nameof(CapturedAt) });
        if (!Enum.IsDefined(Result))
            yield return new ValidationResult("Result is invalid.", new[] { nameof(Result) });
        if (Result == AttemptResult.Failed && !FailureReason.HasValue)
        {
            yield return new ValidationResult(
                "FailureReason is required when AttemptResult is Failed.",
                new[] { nameof(FailureReason) });
        }
        if (FailureReason.HasValue && !Enum.IsDefined(FailureReason.Value))
            yield return new ValidationResult("FailureReason is invalid.", new[] { nameof(FailureReason) });
    }
}
