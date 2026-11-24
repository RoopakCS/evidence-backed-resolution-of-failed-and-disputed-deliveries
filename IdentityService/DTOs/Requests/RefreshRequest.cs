using System.ComponentModel.DataAnnotations;

namespace IdentityService.DTOs.Requests;

public class RefreshRequest
{
    [Required(ErrorMessage = "RefreshToken is required.")]
    public string RefreshToken { get; set; } = string.Empty;
}
