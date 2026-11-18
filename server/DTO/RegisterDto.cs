using System.ComponentModel.DataAnnotations;

namespace server.DTO;

public class RegisterDto
{

    [Required(ErrorMessage = "First Name is Required")]
    [MaxLength(50)]
    public string firstname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last Name is Required")]
    [MaxLength(50)]
    public string lastname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is Required")]
    [EmailAddress(ErrorMessage = "Invalid Email")]
    [MaxLength(100)]
    public string email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone Number is Required")]
    [Phone(ErrorMessage = "Invalid Phone Number")]
    [StringLength(15, MinimumLength = 10, ErrorMessage = "Phone Number must be between 10 and 15 characters")]
    public string phonenumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is Required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must contain at least 1 uppercase, 1 lowercase, 1 number and 1 special character")]
    [DataType(DataType.Password)]
    public string password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm Password")]
    [Compare("password", ErrorMessage = "Password doesn't match")]
    [DataType(DataType.Password)]
    public string confirmpassword { get; set; } = string.Empty;
}
