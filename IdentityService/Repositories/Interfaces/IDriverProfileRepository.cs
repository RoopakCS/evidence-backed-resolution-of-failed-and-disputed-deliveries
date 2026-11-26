using IdentityService.Models;

namespace IdentityService.Repositories.Interfaces;

public interface IDriverProfileRepository
{
    Task<DriverProfile?> GetByUserIdAsync(Guid userId);

    Task UpdateAsync(DriverProfile driverProfile);
}
