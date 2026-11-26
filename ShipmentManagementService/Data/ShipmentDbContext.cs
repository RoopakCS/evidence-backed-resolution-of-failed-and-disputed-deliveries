using Microsoft.EntityFrameworkCore;
using ShipmentManagementService.Models;

namespace ShipmentManagementService.Data;

public class ShipmentDbContext : DbContext
{
    public ShipmentDbContext(DbContextOptions<ShipmentDbContext> options)
        : base(options)
    {
    }

    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<DeliveryInstruction> DeliveryInstructions => Set<DeliveryInstruction>();
    public DbSet<RescheduleRequest> RescheduleRequests => Set<RescheduleRequest>();
    public DbSet<Return> Returns => Set<Return>();
    public DbSet<ShipmentStatusHistory> ShipmentStatusHistories => Set<ShipmentStatusHistory>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.HasIndex(s => s.TrackingNumber).IsUnique();
            entity.HasIndex(s => s.CustomerId);

            entity.Property(s => s.TrackingNumber).HasMaxLength(64).IsRequired();
            entity.Property(s => s.RecipientName).HasMaxLength(150).IsRequired();
            entity.Property(s => s.RecipientPhone).HasMaxLength(30).IsRequired();
            entity.Property(s => s.AddressLine1).HasMaxLength(200).IsRequired();
            entity.Property(s => s.AddressLine2).HasMaxLength(200);
            entity.Property(s => s.City).HasMaxLength(100).IsRequired();
            entity.Property(s => s.State).HasMaxLength(100).IsRequired();
            entity.Property(s => s.PostalCode).HasMaxLength(30).IsRequired();
            entity.Property(s => s.Country).HasMaxLength(100).IsRequired();
            entity.Property(s => s.Status).HasConversion<string>().HasMaxLength(40);
        });

        modelBuilder.Entity<DeliveryInstruction>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.HasIndex(i => i.ShipmentId);

            entity.Property(i => i.InstructionType).HasConversion<string>().HasMaxLength(40);
            entity.Property(i => i.DropOffPlace).HasMaxLength(100).IsRequired();
            entity.Property(i => i.GateOrAccessCode).HasMaxLength(50);
            entity.Property(i => i.Floor).HasMaxLength(30);
            entity.Property(i => i.Door).HasMaxLength(30);
            entity.Property(i => i.InstructionText).HasMaxLength(500);
            entity.Property(i => i.PhotoUrl).HasMaxLength(2048);
            entity.Property(i => i.Status).HasConversion<string>().HasMaxLength(30);

            entity.HasOne(i => i.Shipment)
                .WithMany(s => s.Instructions)
                .HasForeignKey(i => i.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RescheduleRequest>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasIndex(r => r.ShipmentId);

            entity.Property(r => r.Reason).HasMaxLength(500).IsRequired();
            entity.Property(r => r.Status).HasConversion<string>().HasMaxLength(30);

            entity.HasOne(r => r.Shipment)
                .WithMany(s => s.RescheduleRequests)
                .HasForeignKey(r => r.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Return>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasIndex(r => r.ShipmentId);

            entity.Property(r => r.ReturnReason).HasMaxLength(500).IsRequired();
            entity.Property(r => r.Status).HasConversion<string>().HasMaxLength(30);

            entity.HasOne(r => r.Shipment)
                .WithMany(s => s.Returns)
                .HasForeignKey(r => r.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ShipmentStatusHistory>(entity =>
        {
            entity.HasKey(h => h.Id);
            entity.HasIndex(h => new { h.ShipmentId, h.OccurredAt });

            entity.Property(h => h.Status).HasConversion<string>().HasMaxLength(40);
            entity.Property(h => h.Note).HasMaxLength(500);
            entity.Property(h => h.Location).HasMaxLength(200);

            entity.HasOne(h => h.Shipment)
                .WithMany(s => s.StatusHistory)
                .HasForeignKey(h => h.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<IdempotencyRecord>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasIndex(r => new { r.Key, r.UserId, r.Endpoint }).IsUnique();

            entity.Property(r => r.Key).HasMaxLength(128).IsRequired();
            entity.Property(r => r.Endpoint).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.HasIndex(m => m.ProcessedAt);

            entity.Property(m => m.EventType).HasMaxLength(150).IsRequired();
        });
    }
}
