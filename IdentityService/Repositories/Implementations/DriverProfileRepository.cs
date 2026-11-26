using IdentityService.Data;
using IdentityService.Models;
using IdentityService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Repositories.Implementations;

public class DriverProfileRepository : IDriverProfileRepository
{
    private readonly IdentityDbContext _context;

    public DriverProfileRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<DriverProfile?> GetByUserIdAsync(Guid userId)
    {
        return await _context.DriverProfiles
            .FirstOrDefaultAsync(dp => dp.UserId == userId);
    }

    public async Task<bool> ExistsByLicenseNumberAsync(string licenseNumber, Guid? excludeUserId = null)
    {
        var normalized = licenseNumber.Trim();

        return await _context.DriverProfiles
            .AnyAsync(dp =>
                dp.LicenseNumber == normalized &&
                (!excludeUserId.HasValue || dp.UserId != excludeUserId.Value));
    }

    public async Task<DriverProfile> AddAsync(DriverProfile driverProfile)
    {
        await _context.DriverProfiles.AddAsync(driverProfile);

        await _context.SaveChangesAsync();

        return driverProfile;
    }

    public async Task UpdateAsync(DriverProfile driverProfile)
    {
        driverProfile.UpdatedAt = DateTime.UtcNow;

        _context.DriverProfiles.Update(driverProfile);

        await _context.SaveChangesAsync();
    }
}
