using DeliveryAttemptService.DTOs.Requests;
using DeliveryAttemptService.DTOs.Responses;

namespace DeliveryAttemptService.Services.Interfaces;

public interface IEvidenceService
{
    Task<CreateEvidenceUploadUrlResponse> CreateUploadUrlAsync(
        Guid driverId,
        CreateEvidenceUploadUrlRequest request,
        CancellationToken ct = default);

    Task<EvidenceResponse> ConfirmUploadAsync(
        Guid driverId,
        ConfirmEvidenceUploadRequest request,
        CancellationToken ct = default);

    Task<List<EvidenceResponse>> GetEvidenceByAttemptAsync(
        Guid attemptId,
        Guid callerId,
        string callerRole,
        CancellationToken ct = default);
}
