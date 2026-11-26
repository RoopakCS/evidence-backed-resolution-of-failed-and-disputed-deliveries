using System.ComponentModel.DataAnnotations;
using IdentityService.Models.Enums;

namespace IdentityService.DTOs.Requests;

public class UpdateDriverAvailabilityRequest
{
    [Required(ErrorMessage = "AvailabilityStatus is required.")]
    public DriverAvailabilityStatus AvailabilityStatus { get; set; }
}
