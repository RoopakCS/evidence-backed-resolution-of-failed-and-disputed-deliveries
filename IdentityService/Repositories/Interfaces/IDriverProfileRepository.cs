using IdentityService.Models;

namespace IdentityService.Repositories.Interfaces;

public interface IDriverProfileRepository
{
    Task<DriverProfile?> GetByUserIdAsync(Guid userId);

    Task<bool> ExistsByLicenseNumberAsync(string licenseNumber, Guid? excludeUserId = null);

    Task<DriverProfile> AddAsync(DriverProfile driverProfile);

    Task UpdateAsync(DriverProfile driverProfile);
}
