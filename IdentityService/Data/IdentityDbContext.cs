using IdentityService.Models;
using IdentityService.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Data;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<CustomerProfile> CustomerProfiles => Set<CustomerProfile>();

    public DbSet<DriverProfile> DriverProfiles => Set<DriverProfile>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── User ────────────────────────────────────────────────────────────
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");

            entity.Property(u => u.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasIndex(u => u.PhoneNumber)
                .IsUnique()
                .HasDatabaseName("IX_Users_PhoneNumber");

            entity.Property(u => u.PasswordHash)
                .IsRequired();

            entity.Property(u => u.Address)
                .HasMaxLength(500);

            entity.Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(u => u.IsActive)
                .HasDefaultValue(true);

            entity.Property(u => u.CreatedAt)
                .IsRequired();

            entity.Property(u => u.UpdatedAt)
                .IsRequired();

            // 1-to-1: User → CustomerProfile
            entity.HasOne(u => u.CustomerProfile)
                .WithOne(cp => cp.User)
                .HasForeignKey<CustomerProfile>(cp => cp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-1: User → DriverProfile
            entity.HasOne(u => u.DriverProfile)
                .WithOne(dp => dp.User)
                .HasForeignKey<DriverProfile>(dp => dp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-many: User → RefreshTokens
            entity.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ── CustomerProfile ──────────────────────────────────────────────────
        modelBuilder.Entity<CustomerProfile>(entity =>
        {
            entity.HasKey(cp => cp.Id);

            entity.HasIndex(cp => cp.UserId)
                .IsUnique()
                .HasDatabaseName("IX_CustomerProfiles_UserId");

            entity.Property(cp => cp.CreatedAt).IsRequired();
            entity.Property(cp => cp.UpdatedAt).IsRequired();
        });

        // ── DriverProfile ────────────────────────────────────────────────────
        modelBuilder.Entity<DriverProfile>(entity =>
        {
            entity.HasKey(dp => dp.Id);

            entity.HasIndex(dp => dp.UserId)
                .IsUnique()
                .HasDatabaseName("IX_DriverProfiles_UserId");

            entity.Property(dp => dp.LicenseNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(dp => dp.LicenseNumber)
                .IsUnique()
                .HasDatabaseName("IX_DriverProfiles_LicenseNumber");

            entity.Property(dp => dp.VehicleType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(dp => dp.VehicleRegistrationNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(dp => dp.VerificationStatus)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(dp => dp.AvailabilityStatus)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(dp => dp.CreatedAt).IsRequired();
            entity.Property(dp => dp.UpdatedAt).IsRequired();
        });

        // ── RefreshToken ─────────────────────────────────────────────────────
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);

            entity.Property(rt => rt.TokenHash)
                .IsRequired()
                .HasMaxLength(128);

            entity.HasIndex(rt => rt.TokenHash)
                .IsUnique()
                .HasDatabaseName("IX_RefreshTokens_TokenHash");

            // Composite index to quickly look up active tokens per user
            entity.HasIndex(rt => new { rt.UserId, rt.ExpiresAt })
                .HasDatabaseName("IX_RefreshTokens_UserId_ExpiresAt");

            entity.Property(rt => rt.ExpiresAt).IsRequired();
            entity.Property(rt => rt.CreatedAt).IsRequired();

            entity.Property(rt => rt.DeviceClientMetadata)
                .HasMaxLength(500);

            // Computed columns (IsExpired, IsRevoked, IsActive) are
            // ignored — they are computed in C# and not persisted.
            entity.Ignore(rt => rt.IsExpired);
            entity.Ignore(rt => rt.IsRevoked);
            entity.Ignore(rt => rt.IsActive);
        });
    }
}
