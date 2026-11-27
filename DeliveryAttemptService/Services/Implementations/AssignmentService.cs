using DeliveryAttemptService.DTOs.Requests;
using DeliveryAttemptService.DTOs.Responses;
using DeliveryAttemptService.Models;
using DeliveryAttemptService.Repositories.Interfaces;
using DeliveryAttemptService.Services.Interfaces;
using DeliveryAttemptService.Services.Mapping;

namespace DeliveryAttemptService.Services.Implementations;

public class AssignmentService : IAssignmentService
{
    private readonly IAssignmentRepository _assignments;

    public AssignmentService(IAssignmentRepository assignments)
    {
        _assignments = assignments;
    }

    public async Task<List<DriverAssignmentResponse>> GetAssignmentsForDriverAsync(
        Guid driverId,
        DateTime? dateUtc,
        CancellationToken ct = default)
    {
        var targetDate = (dateUtc ?? DateTime.UtcNow).ToUniversalTime().Date;
        var items = await _assignments.GetByDriverAndDateAsync(driverId, targetDate, ct);
        return items.Select(AttemptMapper.ToResponse).ToList();
    }

    public async Task<DriverAssignmentResponse> UpsertAssignmentAsync(
        CreateDriverAssignmentRequest request,
        CancellationToken ct = default)
    {
        var existing = await _assignments.GetByShipmentAsync(request.ShipmentId, ct);
        var scheduledDate = (request.ScheduledDate ?? DateTime.UtcNow).ToUniversalTime().Date;
        var instructionsJson = AttemptMapper.SerializeInstructions(request.Instructions);

        if (existing is not null)
        {
            existing.TrackingNumber = request.TrackingNumber.Trim();
            existing.DriverId = request.DriverId;
            existing.CustomerId = request.CustomerId;
            existing.RecipientName = request.RecipientName.Trim();
            existing.RecipientPhone = request.RecipientPhone.Trim();
            existing.AddressLine1 = request.AddressLine1.Trim();
            existing.AddressLine2 = request.AddressLine2?.Trim();
            existing.City = request.City.Trim();
            existing.State = request.State.Trim();
            existing.PostalCode = request.PostalCode.Trim();
            existing.Country = request.Country.Trim();
            existing.Latitude = request.Latitude;
            existing.Longitude = request.Longitude;
            existing.ScheduledDate = scheduledDate;
            existing.InstructionsJson = instructionsJson;
            existing.UpdatedAt = DateTime.UtcNow;

            await _assignments.SaveChangesAsync(ct);
            return AttemptMapper.ToResponse(existing);
        }

        var assignment = new DriverAssignment
        {
            ShipmentId = request.ShipmentId,
            DriverId = request.DriverId,
            CustomerId = request.CustomerId,
            TrackingNumber = request.TrackingNumber.Trim(),
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
            DeliveryState = "Assigned",
            ScheduledDate = scheduledDate,
            InstructionsJson = instructionsJson,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _assignments.AddAsync(assignment, ct);
        await _assignments.SaveChangesAsync(ct);

        return AttemptMapper.ToResponse(assignment);
    }
}
