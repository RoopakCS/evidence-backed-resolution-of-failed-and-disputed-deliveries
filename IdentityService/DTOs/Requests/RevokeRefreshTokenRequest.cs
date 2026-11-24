using System.ComponentModel.DataAnnotations;

namespace IdentityService.DTOs.Requests;

public class RevokeRefreshTokenRequest
{
    [Required(ErrorMessage = "RefreshToken is required.")]
    public string RefreshToken { get; set; } = string.Empty;
}
