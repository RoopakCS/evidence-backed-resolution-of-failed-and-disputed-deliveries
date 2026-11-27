using DeliveryAttemptService.Models;

namespace DeliveryAttemptService.Repositories.Interfaces;

public interface IAssignmentRepository
{
    Task<List<DriverAssignment>> GetByDriverAndDateAsync(Guid driverId, DateTime dateUtc, CancellationToken ct = default);

    Task<DriverAssignment?> GetByShipmentAndDriverAsync(Guid shipmentId, Guid driverId, CancellationToken ct = default);

    Task<DriverAssignment?> GetByShipmentAsync(Guid shipmentId, CancellationToken ct = default);

    Task<DriverAssignment?> GetByShipmentAndCustomerAsync(
        Guid shipmentId,
        Guid customerId,
        CancellationToken ct = default);

    Task AddAsync(DriverAssignment assignment, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}
