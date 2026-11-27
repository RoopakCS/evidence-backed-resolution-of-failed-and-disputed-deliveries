using DeliveryAttemptService.DTOs.Requests;
using DeliveryAttemptService.DTOs.Responses;
using DeliveryAttemptService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryAttemptService.Controllers;

[ApiController]
[Route("api/driver/assignments")]
[Authorize]
public class AssignmentsController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;

    public AssignmentsController(IAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    [HttpGet]
    [Authorize(Roles = "Driver")]
    public async Task<ActionResult<List<DriverAssignmentResponse>>> GetMyAssignments(
        [FromQuery] DateTime? date,
        CancellationToken ct)
    {
        var driverId = User.GetRequiredUserId();
        var assignments = await _assignmentService.GetAssignmentsForDriverAsync(driverId, date, ct);
        return Ok(assignments);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator,SupportAgent")]
    public async Task<ActionResult<DriverAssignmentResponse>> UpsertAssignment(
        [FromBody] CreateDriverAssignmentRequest request,
        CancellationToken ct)
    {
        var result = await _assignmentService.UpsertAssignmentAsync(request, ct);
        return Ok(result);
    }
}
