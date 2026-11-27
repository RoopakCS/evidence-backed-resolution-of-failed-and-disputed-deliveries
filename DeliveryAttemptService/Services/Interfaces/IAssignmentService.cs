using DeliveryAttemptService.DTOs.Requests;
using DeliveryAttemptService.DTOs.Responses;

namespace DeliveryAttemptService.Services.Interfaces;

public interface IAssignmentService
{
    Task<List<DriverAssignmentResponse>> GetAssignmentsForDriverAsync(Guid driverId, DateTime? dateUtc, CancellationToken ct = default);

    Task<DriverAssignmentResponse> UpsertAssignmentAsync(CreateDriverAssignmentRequest request, CancellationToken ct = default);
}
