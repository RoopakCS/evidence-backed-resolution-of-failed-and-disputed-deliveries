using System.ComponentModel.DataAnnotations;
using ShipmentManagementService.Models.Enums;

namespace ShipmentManagementService.DTOs.Requests;

public class UpdateInstructionRequest
{
    public InstructionType? InstructionType { get; set; }

    [StringLength(100, ErrorMessage = "Drop-off place cannot exceed 100 characters.")]
    public string? DropOffPlace { get; set; }

    [StringLength(50, ErrorMessage = "Gate or access code cannot exceed 50 characters.")]
    public string? GateOrAccessCode { get; set; }

    [StringLength(30, ErrorMessage = "Floor cannot exceed 30 characters.")]
    public string? Floor { get; set; }

    [StringLength(30, ErrorMessage = "Door cannot exceed 30 characters.")]
    public string? Door { get; set; }

    [StringLength(500, ErrorMessage = "Instruction text cannot exceed 500 characters.")]
    public string? InstructionText { get; set; }

    [Url(ErrorMessage = "Photo URL must be a valid URL.")]
    [StringLength(2048, ErrorMessage = "Photo URL cannot exceed 2048 characters.")]
    public string? PhotoUrl { get; set; }
}
