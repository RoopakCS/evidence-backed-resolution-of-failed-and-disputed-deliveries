using DeliveryAttemptService.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAttemptService.Data;

public class DeliveryAttemptDbContext : DbContext
{
    public DeliveryAttemptDbContext(DbContextOptions<DeliveryAttemptDbContext> options)
        : base(options)
    {
    }

    public DbSet<DriverAssignment> DriverAssignments => Set<DriverAssignment>();
    public DbSet<DeliveryAttempt> DeliveryAttempts => Set<DeliveryAttempt>();
    public DbSet<Evidence> EvidenceItems => Set<Evidence>();
    public DbSet<InstructionDecision> InstructionDecisions => Set<InstructionDecision>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DriverAssignment>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.HasIndex(a => a.ShipmentId).IsUnique();
            entity.HasIndex(a => new { a.DriverId, a.ScheduledDate });
            entity.HasIndex(a => a.CustomerId);

            entity.Property(a => a.TrackingNumber).HasMaxLength(64).IsRequired();
            entity.Property(a => a.RecipientName).HasMaxLength(150).IsRequired();
            entity.Property(a => a.RecipientPhone).HasMaxLength(30).IsRequired();
            entity.Property(a => a.AddressLine1).HasMaxLength(200).IsRequired();
            entity.Property(a => a.AddressLine2).HasMaxLength(200);
            entity.Property(a => a.City).HasMaxLength(100).IsRequired();
            entity.Property(a => a.State).HasMaxLength(100).IsRequired();
            entity.Property(a => a.PostalCode).HasMaxLength(30).IsRequired();
            entity.Property(a => a.Country).HasMaxLength(100).IsRequired();
            entity.Property(a => a.DeliveryState).HasMaxLength(40).IsRequired();
            entity.Property(a => a.InstructionsJson).HasColumnType("json").IsRequired();
        });

        modelBuilder.Entity<DeliveryAttempt>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.HasIndex(a => new { a.IdempotencyKey, a.DriverId }).IsUnique();
            entity.HasIndex(a => new { a.ShipmentId, a.CapturedAt });
            entity.HasIndex(a => new { a.DriverId, a.CapturedAt });

            entity.Property(a => a.Result).HasConversion<string>().HasMaxLength(30);
            entity.Property(a => a.FailureReason).HasConversion<string>().HasMaxLength(50);
            entity.Property(a => a.AttemptNote).HasMaxLength(1000);
            entity.Property(a => a.IdempotencyKey).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Evidence>(entity =>
        {
            entity.ToTable("Evidence");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.AttemptId);
            entity.HasIndex(e => e.ObjectKey).IsUnique();

            entity.Property(e => e.ObjectKey).HasMaxLength(500).IsRequired();
            entity.Property(e => e.ContentHash).HasMaxLength(128).IsRequired();
            entity.Property(e => e.MediaType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.EvidenceType).HasConversion<string>().HasMaxLength(40);

            entity.HasOne(e => e.Attempt)
                .WithMany(a => a.EvidenceItems)
                .HasForeignKey(e => e.AttemptId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InstructionDecision>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.HasIndex(d => new { d.IdempotencyKey, d.DriverId }).IsUnique();
            entity.HasIndex(d => new { d.InstructionId, d.DriverId });
            entity.HasIndex(d => d.ShipmentId);

            entity.Property(d => d.Decision).HasConversion<string>().HasMaxLength(30);
            entity.Property(d => d.Reason).HasMaxLength(500);
            entity.Property(d => d.IdempotencyKey).HasMaxLength(100).IsRequired();

            entity.HasOne(d => d.DeliveryAttempt)
                .WithMany(a => a.InstructionDecisions)
                .HasForeignKey(d => d.DeliveryAttemptId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<IdempotencyRecord>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasIndex(r => new { r.IdempotencyKey, r.UserId, r.ActionType }).IsUnique();

            entity.Property(r => r.IdempotencyKey).HasMaxLength(128).IsRequired();
            entity.Property(r => r.Endpoint).HasMaxLength(200).IsRequired();
            entity.Property(r => r.ActionType).HasMaxLength(100).IsRequired();
            entity.Property(r => r.RequestHash).HasMaxLength(128).IsRequired();
        });

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.HasIndex(m => m.ProcessedAt);

            entity.Property(m => m.EventType).HasMaxLength(150).IsRequired();
            entity.Property(m => m.Payload).IsRequired();
        });
    }
}
