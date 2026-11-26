using System.ComponentModel.DataAnnotations;
using TicketManagementService.Models.Enums;

namespace TicketManagementService.DTOs.Requests;

public class UpdateTicketStatusRequest : IValidatableObject
{
    [Required(ErrorMessage = "TargetStatus is required.")]
    public TicketStatus TargetStatus { get; set; }

    [StringLength(2000, ErrorMessage = "Reason cannot exceed 2000 characters.")]
    public string? Reason { get; set; }

    /// <summary>
    /// Resolution description. Required when transitioning to Resolved status.
    /// </summary>
    [StringLength(4000, ErrorMessage = "Resolution cannot exceed 4000 characters.")]
    public string? Resolution { get; set; }

    /// <summary>
    /// Optional expected version for optimistic concurrency control.
    /// </summary>
    public int? ExpectedVersion { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (TargetStatus == TicketStatus.Resolved && string.IsNullOrWhiteSpace(Resolution))
        {
            yield return new ValidationResult(
                "Resolution is required when transitioning to Resolved status.",
                new[] { nameof(Resolution) });
        }
    }
}
