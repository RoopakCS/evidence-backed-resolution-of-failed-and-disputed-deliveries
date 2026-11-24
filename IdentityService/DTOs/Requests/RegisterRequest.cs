using System.ComponentModel.DataAnnotations;
using IdentityService.Models.Enums;

namespace IdentityService.DTOs.Requests;

public class RegisterRequest : IValidatableObject
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [Phone(ErrorMessage = "Invalid phone number format.")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required.")]
    public Role Role { get; set; } = Role.Customer;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Role != Role.Customer && Role != Role.Driver)
        {
            yield return new ValidationResult(
                "Self-registration is restricted to Customer and Driver roles only. Privileged roles require administrative provisioning.",
                new[] { nameof(Role) });
        }
    }
}
