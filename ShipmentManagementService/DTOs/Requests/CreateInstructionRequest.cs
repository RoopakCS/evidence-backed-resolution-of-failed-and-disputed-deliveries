using System.ComponentModel.DataAnnotations;
using ShipmentManagementService.Models.Enums;

namespace ShipmentManagementService.DTOs.Requests;

public class CreateInstructionRequest
{
    [Required(ErrorMessage = "Instruction type is required.")]
    public InstructionType InstructionType { get; set; }

    [Required(ErrorMessage = "Instruction text is required.")]
    [StringLength(500, ErrorMessage = "Instruction text cannot exceed 500 characters.")]
    public string InstructionText { get; set; } = string.Empty;
}
