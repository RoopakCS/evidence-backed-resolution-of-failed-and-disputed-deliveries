using System.ComponentModel.DataAnnotations;
using IdentityService.Models.Enums;

namespace IdentityService.DTOs.Requests;

public class UpdateDriverVerificationRequest
{
    [Required(ErrorMessage = "VerificationStatus is required.")]
    public DriverVerificationStatus VerificationStatus { get; set; }
}
