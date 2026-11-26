using Microsoft.EntityFrameworkCore;
using ShipmentManagementService.Data;
using ShipmentManagementService.Models;
using ShipmentManagementService.Repositories.Interfaces;

namespace ShipmentManagementService.Repositories.Implementations;

public class ShipmentRepository : IShipmentRepository
{
    private readonly ShipmentDbContext _db;

    public ShipmentRepository(ShipmentDbContext db)
    {
        _db = db;
    }

    public async Task<Shipment?> GetByIdAsync(Guid id, bool includeDetails = false, CancellationToken ct = default)
    {
        IQueryable<Shipment> query = _db.Shipments;

        if (includeDetails)
        {
            query = query
                .Include(s => s.Instructions.OrderByDescending(i => i.CreatedAt))
                .Include(s => s.StatusHistory.OrderByDescending(h => h.OccurredAt));
        }

        return await query.FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber, bool includeHistory = false, CancellationToken ct = default)
    {
        IQueryable<Shipment> query = _db.Shipments;

        if (includeHistory)
        {
            query = query.Include(s => s.StatusHistory.OrderByDescending(h => h.OccurredAt));
        }

        return await query.FirstOrDefaultAsync(s => s.TrackingNumber == trackingNumber, ct);
    }

    public async Task<List<Shipment>> GetByCustomerIdAsync(Guid customerId, int page, int pageSize, CancellationToken ct = default)
    {
        return await _db.Shipments
            .AsNoTracking()
            .Where(s => s.CustomerId == customerId)
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<List<Shipment>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        return await _db.Shipments
            .AsNoTracking()
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public Task<bool> TrackingNumberExistsAsync(string trackingNumber, CancellationToken ct = default)
    {
        return _db.Shipments.AnyAsync(s => s.TrackingNumber == trackingNumber, ct);
    }

    public async Task AddAsync(Shipment shipment, CancellationToken ct = default)
    {
        await _db.Shipments.AddAsync(shipment, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
    {
        return _db.SaveChangesAsync(ct);
    }
}
