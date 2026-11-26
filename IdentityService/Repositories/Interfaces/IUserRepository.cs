using IdentityService.Models;

namespace IdentityService.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);

    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByPhoneNumberAsync(string phoneNumber);

    /// <summary>
    /// Looks up a user by email OR phone number — supports the
    /// flexible LoginRequest identifier.
    /// </summary>
    Task<User?> GetByEmailOrPhoneAsync(string identifier);

    Task<bool> ExistsByEmailAsync(string email);

    Task<bool> ExistsByPhoneNumberAsync(string phoneNumber);

    Task<User> AddAsync(User user);

    Task UpdateAsync(User user);
}
