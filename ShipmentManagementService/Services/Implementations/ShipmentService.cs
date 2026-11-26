using System.Security.Cryptography;
using ShipmentManagementService.Data;
using ShipmentManagementService.DTOs.Requests;
using ShipmentManagementService.DTOs.Responses;
using ShipmentManagementService.Exceptions;
using ShipmentManagementService.Models;
using ShipmentManagementService.Models.Enums;
using ShipmentManagementService.Repositories.Interfaces;
using ShipmentManagementService.Services.Interfaces;
using ShipmentManagementService.Services.Mapping;

namespace ShipmentManagementService.Services.Implementations;

public class ShipmentService : IShipmentService
{
    private readonly IShipmentRepository _shipments;
    private readonly ShipmentDbContext _db;

    public ShipmentService(IShipmentRepository shipments, ShipmentDbContext db)
    {
        _shipments = shipments;
        _db = db;
    }

    public async Task<ShipmentResponse> CreateAsync(
        CreateShipmentRequest request,
        Guid actorUserId,
        string actorRole,
        CancellationToken ct = default)
    {
        var customerId = IsStaff(actorRole) && request.CustomerId.HasValue
            ? request.CustomerId.Value
            : actorUserId;

        var now = DateTime.UtcNow;
        var trackingNumber = await GenerateUniqueTrackingNumberAsync(ct);

        var shipment = new Shipment
        {
            TrackingNumber = trackingNumber,
            CustomerId = customerId,
            RecipientName = request.RecipientName.Trim(),
            RecipientPhone = request.RecipientPhone.Trim(),
            AddressLine1 = request.AddressLine1.Trim(),
            AddressLine2 = request.AddressLine2?.Trim(),
            City = request.City.Trim(),
            State = request.State.Trim(),
            PostalCode = request.PostalCode.Trim(),
            Country = request.Country.Trim(),
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Status = ShipmentStatus.Created,
            ExpectedDeliveryDate = DateTime.SpecifyKind(request.ExpectedDeliveryDate, DateTimeKind.Utc),
            CreatedAt = now,
            UpdatedAt = now
        };

        shipment.StatusHistory.Add(new ShipmentStatusHistory
        {
            ShipmentId = shipment.Id,
            Status = ShipmentStatus.Created,
            Note = "Shipment created.",
            RecordedByUserId = actorUserId,
            OccurredAt = now
        });

        if (request.InitialInstructions is { Count: > 0 })
        {
            foreach (var instr in request.InitialInstructions)
            {
                shipment.Instructions.Add(new DeliveryInstruction
                {
                    ShipmentId = shipment.Id,
                    InstructionType = instr.InstructionType,
                    DropOffPlace = instr.DropOffPlace.Trim(),
                    GateOrAccessCode = instr.GateOrAccessCode?.Trim(),
                    Floor = instr.Floor?.Trim(),
                    Door = instr.Door?.Trim(),
                    InstructionText = instr.InstructionText?.Trim(),
                    PhotoUrl = instr.PhotoUrl?.Trim(),
                    Status = InstructionStatus.Pending,
                    CreatedAt = now,
                    UpdatedAt = now
                });
            }
        }

        await _shipments.AddAsync(shipment, ct);
        await _shipments.SaveChangesAsync(ct);

        return ShipmentMapper.ToResponse(shipment, includeInstructions: true);
    }

    public async Task<ShipmentResponse> GetByIdAsync(
        Guid id,
        Guid actorUserId,
        string actorRole,
        CancellationToken ct = default)
    {
        var shipment = await _shipments.GetByIdAsync(id, includeDetails: true, ct)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, "Shipment not found.");

        EnsureReadAccess(shipment, actorUserId, actorRole);

        return ShipmentMapper.ToResponse(shipment, includeInstructions: true);
    }

    public async Task<List<ShipmentSummaryResponse>> ListAsync(
        Guid actorUserId,
        string actorRole,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var normalizedPage = Math.Max(1, page);
        var normalizedSize = Math.Clamp(pageSize, 1, 100);

        var items = IsStaff(actorRole) || IsDriver(actorRole)
            ? await _shipments.GetAllAsync(normalizedPage, normalizedSize, ct)
            : await _shipments.GetByCustomerIdAsync(actorUserId, normalizedPage, normalizedSize, ct);

        return items.Select(ShipmentMapper.ToSummaryResponse).ToList();
    }

    public async Task<PublicTrackingResponse> TrackByNumberAsync(string trackingNumber, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
            throw new ServiceException(StatusCodes.Status400BadRequest, "Tracking number is required.");

        var normalized = trackingNumber.Trim().ToUpperInvariant();
        var shipment = await _shipments.GetByTrackingNumberAsync(normalized, includeHistory: true, ct)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, "Shipment not found.");

        return ShipmentMapper.ToPublicTrackingResponse(shipment);
    }

    public async Task<ShipmentResponse> UpdateAsync(
        Guid id,
        UpdateShipmentRequest request,
        Guid actorUserId,
        string actorRole,
        CancellationToken ct = default)
    {
        var shipment = await _shipments.GetByIdAsync(id, includeDetails: true, ct)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, "Shipment not found.");

        EnsureWriteAccess(shipment, actorUserId, actorRole);

        if (IsTerminalOrLocked(shipment.Status))
            throw new ServiceException(StatusCodes.Status409Conflict,
                $"Shipment cannot be modified in status '{shipment.Status}'.");

        if (request.RecipientName is not null) shipment.RecipientName = request.RecipientName.Trim();
        if (request.RecipientPhone is not null) shipment.RecipientPhone = request.RecipientPhone.Trim();
        if (request.AddressLine1 is not null) shipment.AddressLine1 = request.AddressLine1.Trim();
        if (request.AddressLine2 is not null) shipment.AddressLine2 = request.AddressLine2.Trim();
        if (request.City is not null) shipment.City = request.City.Trim();
        if (request.State is not null) shipment.State = request.State.Trim();
        if (request.PostalCode is not null) shipment.PostalCode = request.PostalCode.Trim();
        if (request.Country is not null) shipment.Country = request.Country.Trim();
        if (request.Latitude.HasValue) shipment.Latitude = request.Latitude;
        if (request.Longitude.HasValue) shipment.Longitude = request.Longitude;
        if (request.ExpectedDeliveryDate.HasValue)
            shipment.ExpectedDeliveryDate = DateTime.SpecifyKind(request.ExpectedDeliveryDate.Value, DateTimeKind.Utc);

        shipment.UpdatedAt = DateTime.UtcNow;

        await _shipments.SaveChangesAsync(ct);

        return ShipmentMapper.ToResponse(shipment, includeInstructions: true);
    }

    public async Task<ShipmentStatusHistoryResponse> RecordTrackingUpdateAsync(
        Guid id,
        RecordTrackingUpdateRequest request,
        Guid actorUserId,
        CancellationToken ct = default)
    {
        var shipment = await _shipments.GetByIdAsync(id, includeDetails: false, ct)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, "Shipment not found.");

        var occurredAt = request.OccurredAt.HasValue
            ? DateTime.SpecifyKind(request.OccurredAt.Value, DateTimeKind.Utc)
            : DateTime.UtcNow;

        shipment.Status = request.Status;
        shipment.UpdatedAt = DateTime.UtcNow;

        var entry = new ShipmentStatusHistory
        {
            ShipmentId = shipment.Id,
            Status = request.Status,
            Note = request.Note?.Trim(),
            Location = request.Location?.Trim(),
            RecordedByUserId = actorUserId,
            OccurredAt = occurredAt
        };

        await _db.ShipmentStatusHistories.AddAsync(entry, ct);
        await _db.SaveChangesAsync(ct);

        return ShipmentMapper.ToResponse(entry);
    }

    public async Task<List<ShipmentStatusHistoryResponse>> GetStatusHistoryAsync(
        Guid id,
        Guid actorUserId,
        string actorRole,
        CancellationToken ct = default)
    {
        var shipment = await _shipments.GetByIdAsync(id, includeDetails: true, ct)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, "Shipment not found.");

        EnsureReadAccess(shipment, actorUserId, actorRole);

        return shipment.StatusHistory
            .OrderByDescending(h => h.OccurredAt)
            .Select(ShipmentMapper.ToResponse)
            .ToList();
    }

    internal static void EnsureReadAccess(Shipment shipment, Guid actorUserId, string actorRole)
    {
        if (IsStaff(actorRole) || IsDriver(actorRole))
            return;

        if (shipment.CustomerId != actorUserId)
            throw new ServiceException(StatusCodes.Status403Forbidden, "You do not have access to this shipment.");
    }

    internal static void EnsureWriteAccess(Shipment shipment, Guid actorUserId, string actorRole)
    {
        if (IsStaff(actorRole))
            return;

        if (shipment.CustomerId != actorUserId)
            throw new ServiceException(StatusCodes.Status403Forbidden, "You do not have permission to modify this shipment.");
    }

    internal static bool IsTerminalOrLocked(ShipmentStatus status) =>
        status is ShipmentStatus.Delivered
            or ShipmentStatus.ReturnInitiated
            or ShipmentStatus.Returned
            or ShipmentStatus.Cancelled;

    private static bool IsStaff(string role) =>
        string.Equals(role, "SupportAgent", StringComparison.OrdinalIgnoreCase)
        || string.Equals(role, "Administrator", StringComparison.OrdinalIgnoreCase);

    private static bool IsDriver(string role) =>
        string.Equals(role, "Driver", StringComparison.OrdinalIgnoreCase);

    private async Task<string> GenerateUniqueTrackingNumberAsync(CancellationToken ct)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        for (var attempt = 0; attempt < 5; attempt++)
        {
            var suffix = RandomNumberGenerator.GetString(chars, 10);
            var candidate = $"DKT-{suffix}";
            if (!await _shipments.TrackingNumberExistsAsync(candidate, ct))
                return candidate;
        }

        return $"DKT-{Guid.NewGuid():N}"[..16].ToUpperInvariant();
    }
}
