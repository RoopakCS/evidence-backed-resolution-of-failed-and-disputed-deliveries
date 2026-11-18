using System.ComponentModel.DataAnnotations;

namespace server.Models;

public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;
}