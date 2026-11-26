using ShipmentManagementService.DTOs.Responses;
using ShipmentManagementService.Models;

namespace ShipmentManagementService.Services.Mapping;

internal static class ShipmentMapper
{
    public static ShipmentResponse ToResponse(Shipment s, bool includeInstructions = false) => new()
    {
        Id = s.Id,
        TrackingNumber = s.TrackingNumber,
        CustomerId = s.CustomerId,
        RecipientName = s.RecipientName,
        RecipientPhone = s.RecipientPhone,
        AddressLine1 = s.AddressLine1,
        AddressLine2 = s.AddressLine2,
        City = s.City,
        State = s.State,
        PostalCode = s.PostalCode,
        Country = s.Country,
        Latitude = s.Latitude,
        Longitude = s.Longitude,
        Status = s.Status,
        ExpectedDeliveryDate = s.ExpectedDeliveryDate,
        CreatedAt = s.CreatedAt,
        UpdatedAt = s.UpdatedAt,
        Instructions = includeInstructions
            ? s.Instructions.Select(ToResponse).ToList()
            : new List<InstructionResponse>()
    };

    public static ShipmentSummaryResponse ToSummaryResponse(Shipment s) => new()
    {
        Id = s.Id,
        TrackingNumber = s.TrackingNumber,
        CustomerId = s.CustomerId,
        RecipientName = s.RecipientName,
        City = s.City,
        State = s.State,
        PostalCode = s.PostalCode,
        Country = s.Country,
        Status = s.Status,
        ExpectedDeliveryDate = s.ExpectedDeliveryDate,
        CreatedAt = s.CreatedAt
    };

    public static PublicTrackingResponse ToPublicTrackingResponse(Shipment s)
    {
        var history = s.StatusHistory
            .OrderByDescending(h => h.OccurredAt)
            .ToList();

        var statusUpdatedAt = history.FirstOrDefault()?.OccurredAt ?? s.UpdatedAt;
        var statusAge = DateTime.UtcNow - statusUpdatedAt;
        if (statusAge < TimeSpan.Zero)
            statusAge = TimeSpan.Zero;

        return new PublicTrackingResponse
        {
            TrackingNumber = s.TrackingNumber,
            Status = s.Status,
            StatusUpdatedAt = statusUpdatedAt,
            StatusAge = statusAge,
            ExpectedDeliveryDate = s.ExpectedDeliveryDate,
            City = s.City,
            State = s.State,
            Country = s.Country,
            History = history.Select(ToResponse).ToList()
        };
    }

    public static ShipmentStatusHistoryResponse ToResponse(ShipmentStatusHistory h) => new()
    {
        Id = h.Id,
        Status = h.Status,
        Note = h.Note,
        Location = h.Location,
        OccurredAt = h.OccurredAt
    };

    public static InstructionResponse ToResponse(DeliveryInstruction i) => new()
    {
        Id = i.Id,
        ShipmentId = i.ShipmentId,
        Type = i.InstructionType,
        DropOffPlace = i.DropOffPlace,
        GateOrAccessCode = i.GateOrAccessCode,
        Floor = i.Floor,
        Door = i.Door,
        Text = i.InstructionText,
        PhotoUrl = i.PhotoUrl,
        Status = i.Status,
        CreatedAt = i.CreatedAt,
        UpdatedAt = i.UpdatedAt
    };

    public static RescheduleResponse ToResponse(RescheduleRequest r) => new()
    {
        RequestId = r.Id,
        ShipmentId = r.ShipmentId,
        RequestedDeliveryDate = r.RequestedDeliveryDate,
        Reason = r.Reason,
        Status = r.Status,
        RequestedAt = r.RequestedAt,
        ProcessedAt = r.ProcessedAt,
        ProcessedByUserId = r.ProcessedByUserId
    };

    public static ReturnResponse ToResponse(Return r) => new()
    {
        ReturnId = r.Id,
        ShipmentId = r.ShipmentId,
        ReturnReason = r.ReturnReason,
        ReturnStatus = r.Status,
        InitiatedAt = r.InitiatedAt,
        CompletedAt = r.CompletedAt,
        UpdatedAt = r.UpdatedAt
    };
}
