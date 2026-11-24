using System.ComponentModel.DataAnnotations;

namespace ShipmentManagementService.DTOs.Requests;

public class RescheduleRequestDto
{
    [Required(ErrorMessage = "Requested delivery date is required.")]
    public DateTime RequestedDeliveryDate { get; set; }

    [Required(ErrorMessage = "Reason is required.")]
    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
    public string Reason { get; set; } = string.Empty;
}
