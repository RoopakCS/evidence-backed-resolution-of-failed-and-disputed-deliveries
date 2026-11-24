using System.ComponentModel.DataAnnotations;

namespace IdentityService.DTOs.Requests;

public class LoginRequest : IValidatableObject
{
    /// <summary>
    /// User identifier: either Email address or PhoneNumber.
    /// </summary>
    public string? EmailOrPhone { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;

    public string GetNormalizedIdentifier()
    {
        if (!string.IsNullOrWhiteSpace(EmailOrPhone))
            return EmailOrPhone.Trim();
        if (!string.IsNullOrWhiteSpace(Email))
            return Email.Trim();
        if (!string.IsNullOrWhiteSpace(PhoneNumber))
            return PhoneNumber.Trim();
        return string.Empty;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(EmailOrPhone) &&
            string.IsNullOrWhiteSpace(Email) &&
            string.IsNullOrWhiteSpace(PhoneNumber))
        {
            yield return new ValidationResult(
                "An Email or PhoneNumber must be provided.",
                new[] { nameof(EmailOrPhone), nameof(Email), nameof(PhoneNumber) });
        }
    }
}
