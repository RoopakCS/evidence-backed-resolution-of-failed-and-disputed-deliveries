using DeliveryAttemptService.Data;
using DeliveryAttemptService.Models;
using DeliveryAttemptService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAttemptService.Repositories.Implementations;

public class AssignmentRepository : IAssignmentRepository
{
    private readonly DeliveryAttemptDbContext _db;

    public AssignmentRepository(DeliveryAttemptDbContext db)
    {
        _db = db;
    }

    public Task<List<DriverAssignment>> GetByDriverAndDateAsync(Guid driverId, DateTime dateUtc, CancellationToken ct = default)
    {
        var dayStart = dateUtc.Date;
        var dayEnd = dayStart.AddDays(1);

        return _db.DriverAssignments
            .AsNoTracking()
            .Where(a => a.DriverId == driverId && a.ScheduledDate >= dayStart && a.ScheduledDate < dayEnd)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync(ct);
    }

    public Task<DriverAssignment?> GetByShipmentAndDriverAsync(Guid shipmentId, Guid driverId, CancellationToken ct = default)
    {
        return _db.DriverAssignments
            .FirstOrDefaultAsync(a => a.ShipmentId == shipmentId && a.DriverId == driverId, ct);
    }

    public Task<DriverAssignment?> GetByShipmentAsync(Guid shipmentId, CancellationToken ct = default)
    {
        return _db.DriverAssignments
            .FirstOrDefaultAsync(a => a.ShipmentId == shipmentId, ct);
    }

    public Task<DriverAssignment?> GetByShipmentAndCustomerAsync(
        Guid shipmentId,
        Guid customerId,
        CancellationToken ct = default)
    {
        return _db.DriverAssignments
            .AsNoTracking()
            .FirstOrDefaultAsync(
                a => a.ShipmentId == shipmentId && a.CustomerId == customerId,
                ct);
    }

    public Task AddAsync(DriverAssignment assignment, CancellationToken ct = default)
    {
        return _db.DriverAssignments.AddAsync(assignment, ct).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
    {
        return _db.SaveChangesAsync(ct);
    }
}
