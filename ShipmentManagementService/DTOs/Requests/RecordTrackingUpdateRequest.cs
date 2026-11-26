using System.ComponentModel.DataAnnotations;
using ShipmentManagementService.Models.Enums;

namespace ShipmentManagementService.DTOs.Requests;

public class RecordTrackingUpdateRequest
{
    [Required(ErrorMessage = "Status is required.")]
    public ShipmentStatus Status { get; set; }

    [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters.")]
    public string? Note { get; set; }

    [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
    public string? Location { get; set; }

    public DateTime? OccurredAt { get; set; }
}
