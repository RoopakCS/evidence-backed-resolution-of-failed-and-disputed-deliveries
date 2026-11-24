using System.ComponentModel.DataAnnotations;

namespace ShipmentManagementService.DTOs.Requests;

public class InitiateReturnRequest
{
    [Required(ErrorMessage = "Return reason is required.")]
    [StringLength(500, ErrorMessage = "Return reason cannot exceed 500 characters.")]
    public string ReturnReason { get; set; } = string.Empty;
}
