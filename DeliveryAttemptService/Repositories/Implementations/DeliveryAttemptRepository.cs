using DeliveryAttemptService.Data;
using DeliveryAttemptService.Models;
using DeliveryAttemptService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAttemptService.Repositories.Implementations;

public class DeliveryAttemptRepository : IDeliveryAttemptRepository
{
    private readonly DeliveryAttemptDbContext _db;

    public DeliveryAttemptRepository(DeliveryAttemptDbContext db)
    {
        _db = db;
    }

    public Task<DeliveryAttempt?> GetAttemptByIdAsync(Guid attemptId, bool includeEvidence = true, CancellationToken ct = default)
    {
        IQueryable<DeliveryAttempt> query = _db.DeliveryAttempts.AsNoTracking();
        if (includeEvidence)
        {
            query = query.Include(a => a.EvidenceItems);
        }

        return query.FirstOrDefaultAsync(a => a.Id == attemptId, ct);
    }

    public Task<List<DeliveryAttempt>> GetAttemptsByShipmentAsync(Guid shipmentId, CancellationToken ct = default)
    {
        return _db.DeliveryAttempts
            .AsNoTracking()
            .Include(a => a.EvidenceItems)
            .Where(a => a.ShipmentId == shipmentId)
            .OrderByDescending(a => a.CapturedAt)
            .ToListAsync(ct);
    }

    public Task AddAttemptAsync(DeliveryAttempt attempt, CancellationToken ct = default)
    {
        return _db.DeliveryAttempts.AddAsync(attempt, ct).AsTask();
    }

    public Task AddInstructionDecisionAsync(InstructionDecision decision, CancellationToken ct = default)
    {
        return _db.InstructionDecisions.AddAsync(decision, ct).AsTask();
    }

    public Task<List<InstructionDecision>> GetDecisionsByShipmentAsync(Guid shipmentId, CancellationToken ct = default)
    {
        return _db.InstructionDecisions
            .AsNoTracking()
            .Where(d => d.ShipmentId == shipmentId)
            .OrderByDescending(d => d.DecidedAt)
            .ToListAsync(ct);
    }

    public Task<Evidence?> GetEvidenceByIdAsync(Guid evidenceId, CancellationToken ct = default)
    {
        return _db.EvidenceItems.FirstOrDefaultAsync(e => e.Id == evidenceId, ct);
    }

    public Task<List<Evidence>> GetEvidenceByAttemptIdAsync(Guid attemptId, CancellationToken ct = default)
    {
        return _db.EvidenceItems
            .AsNoTracking()
            .Where(e => e.AttemptId == attemptId)
            .OrderBy(e => e.UploadedAt)
            .ToListAsync(ct);
    }

    public Task AddEvidenceAsync(Evidence evidence, CancellationToken ct = default)
    {
        return _db.EvidenceItems.AddAsync(evidence, ct).AsTask();
    }

    public Task AddOutboxMessageAsync(OutboxMessage message, CancellationToken ct = default)
    {
        return _db.OutboxMessages.AddAsync(message, ct).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
    {
        return _db.SaveChangesAsync(ct);
    }
}
