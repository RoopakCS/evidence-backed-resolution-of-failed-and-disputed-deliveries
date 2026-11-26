using IdentityService.Data;
using IdentityService.Models;
using IdentityService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _context;

    public UserRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.CustomerProfile)
            .Include(u => u.DriverProfile)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var normalized = email.Trim().ToLowerInvariant();

        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == normalized);
    }

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
    {
        var normalized = phoneNumber.Trim();

        return await _context.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == normalized);
    }

    public async Task<User?> GetByEmailOrPhoneAsync(string identifier)
    {
        var normalized = identifier.Trim();

        // Try email first (contains '@'), then phone
        return await _context.Users
            .FirstOrDefaultAsync(u =>
                u.Email == normalized.ToLowerInvariant() ||
                u.PhoneNumber == normalized);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        var normalized = email.Trim().ToLowerInvariant();

        return await _context.Users
            .AnyAsync(u => u.Email == normalized);
    }

    public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber)
    {
        var normalized = phoneNumber.Trim();

        return await _context.Users
            .AnyAsync(u => u.PhoneNumber == normalized);
    }

    public async Task<User> AddAsync(User user)
    {
        await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;

        _context.Users.Update(user);

        await _context.SaveChangesAsync();
    }
}
