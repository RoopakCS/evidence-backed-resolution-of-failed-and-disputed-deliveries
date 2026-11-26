using ShipmentManagementService.Models;

namespace ShipmentManagementService.Repositories.Interfaces;

public interface IShipmentRepository
{
    Task<Shipment?> GetByIdAsync(Guid id, bool includeDetails = false, CancellationToken ct = default);
    Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber, bool includeHistory = false, CancellationToken ct = default);
    Task<List<Shipment>> GetByCustomerIdAsync(Guid customerId, int page, int pageSize, CancellationToken ct = default);
    Task<List<Shipment>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<bool> TrackingNumberExistsAsync(string trackingNumber, CancellationToken ct = default);
    Task AddAsync(Shipment shipment, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
