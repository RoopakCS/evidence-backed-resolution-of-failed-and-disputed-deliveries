using Docket.Security.Claims;
using IdentityService.DTOs.Requests;
using IdentityService.DTOs.Responses;
using IdentityService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[ApiController]
[Route("api/drivers")]
[Authorize]
public class DriverController : ControllerBase
{
    private readonly IDriverService _driverService;

    public DriverController(IDriverService driverService)
    {
        _driverService = driverService;
    }

    [HttpGet("me")]
    [Authorize(Roles = "Driver")]
    public async Task<ActionResult<DriverProfileResponse>> GetMyDriverProfile()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var response = await _driverService.GetDriverProfileAsync(userId.Value);
        return Ok(response);
    }

    [HttpPut("me")]
    [Authorize(Roles = "Driver")]
    public async Task<ActionResult<DriverProfileResponse>> UpsertMyDriverProfile(
        [FromBody] DriverProfileRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var response = await _driverService.UpsertDriverProfileAsync(userId.Value, request);
        return Ok(response);
    }

    [HttpPut("me/availability")]
    [Authorize(Roles = "Driver")]
    public async Task<ActionResult<DriverProfileResponse>> UpdateMyAvailability(
        [FromBody] UpdateDriverAvailabilityRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var response = await _driverService.UpdateAvailabilityAsync(
            userId.Value,
            request.AvailabilityStatus);
        return Ok(response);
    }

    [HttpPut("{driverUserId:guid}/verify")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<DriverProfileResponse>> VerifyDriver(
        Guid driverUserId,
        [FromBody] UpdateDriverVerificationRequest request)
    {
        var response = await _driverService.UpdateVerificationStatusAsync(
            driverUserId,
            request.VerificationStatus);
        return Ok(response);
    }

    private Guid? GetCurrentUserId()
    {
        var rawId = User.GetUserId();
        return Guid.TryParse(rawId, out var userId) ? userId : null;
    }
}
