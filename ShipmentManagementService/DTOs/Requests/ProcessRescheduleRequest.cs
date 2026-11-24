using System.ComponentModel.DataAnnotations;

namespace ShipmentManagementService.DTOs.Requests;

public class ProcessRescheduleRequest : IValidatableObject
{
    /// <summary>
    /// Decision: either "Approve" or "Reject".
    /// </summary>
    [Required(ErrorMessage = "Decision is required (Approve or Reject).")]
    public string Decision { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
    public string? Reason { get; set; }

    public bool IsApproved => string.Equals(Decision, "Approve", StringComparison.OrdinalIgnoreCase);

    public bool IsRejected => string.Equals(Decision, "Reject", StringComparison.OrdinalIgnoreCase);

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!IsApproved && !IsRejected)
        {
            yield return new ValidationResult(
                "Decision must be either 'Approve' or 'Reject'.",
                new[] { nameof(Decision) });
        }
    }
}
