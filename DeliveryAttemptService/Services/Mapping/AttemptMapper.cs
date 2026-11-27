using System.Text.Json;
using DeliveryAttemptService.DTOs.Common;
using DeliveryAttemptService.DTOs.Responses;
using DeliveryAttemptService.Models;

namespace DeliveryAttemptService.Services.Mapping;

internal static class AttemptMapper
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static DriverAssignmentResponse ToResponse(DriverAssignment assignment)
    {
        var instructions = string.IsNullOrWhiteSpace(assignment.InstructionsJson)
            ? new List<DriverInstructionSummaryDto>()
            : JsonSerializer.Deserialize<List<DriverInstructionSummaryDto>>(assignment.InstructionsJson, JsonOptions)
              ?? new List<DriverInstructionSummaryDto>();

        return new DriverAssignmentResponse
        {
            ShipmentId = assignment.ShipmentId,
            TrackingNumber = assignment.TrackingNumber,
            RecipientName = assignment.RecipientName,
            RecipientPhone = assignment.RecipientPhone,
            AddressLine1 = assignment.AddressLine1,
            AddressLine2 = assignment.AddressLine2,
            City = assignment.City,
            State = assignment.State,
            PostalCode = assignment.PostalCode,
            Country = assignment.Country,
            Latitude = assignment.Latitude,
            Longitude = assignment.Longitude,
            DeliveryState = assignment.DeliveryState,
            Instructions = instructions
        };
    }

    public static InstructionDecisionResponse ToResponse(InstructionDecision decision)
    {
        return new InstructionDecisionResponse
        {
            DecisionId = decision.Id,
            InstructionId = decision.InstructionId,
            ShipmentId = decision.ShipmentId,
            DriverId = decision.DriverId,
            Decision = decision.Decision,
            Reason = decision.Reason,
            DecidedAt = decision.DecidedAt,
            CreatedAt = decision.CreatedAt
        };
    }

    public static AttemptResponse ToResponse(DeliveryAttempt attempt, Func<Evidence, string?>? downloadUrlFactory = null)
    {
        return new AttemptResponse
        {
            AttemptId = attempt.Id,
            ShipmentId = attempt.ShipmentId,
            DriverId = attempt.DriverId,
            Result = attempt.Result,
            FailureReason = attempt.FailureReason,
            AttemptNote = attempt.AttemptNote,
            Latitude = attempt.Latitude,
            Longitude = attempt.Longitude,
            CapturedAt = attempt.CapturedAt,
            ReceivedAt = attempt.ReceivedAt,
            CreatedAt = attempt.CreatedAt,
            Evidence = attempt.EvidenceItems
                .Select(e => ToResponse(e, downloadUrlFactory?.Invoke(e)))
                .ToList()
        };
    }

    public static EvidenceResponse ToResponse(Evidence evidence, string? downloadUrl = null)
    {
        return new EvidenceResponse
        {
            EvidenceId = evidence.Id,
            AttemptId = evidence.AttemptId,
            EvidenceType = evidence.EvidenceType,
            MediaType = evidence.MediaType,
            ContentHash = evidence.ContentHash,
            UploadedAt = evidence.UploadedAt,
            DownloadUrl = downloadUrl
        };
    }

    public static string SerializeInstructions(List<DriverInstructionSummaryDto>? instructions)
    {
        return JsonSerializer.Serialize(instructions ?? new List<DriverInstructionSummaryDto>(), JsonOptions);
    }
}
