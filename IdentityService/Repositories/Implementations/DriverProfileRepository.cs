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

    public async Task UpdateAsync(DriverProfile driverProfile)
    {
        driverProfile.UpdatedAt = DateTime.UtcNow;

        _context.DriverProfiles.Update(driverProfile);

        await _context.SaveChangesAsync();
    }
}
