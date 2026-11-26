using Docket.Security.Claims;
using IdentityService.DTOs.Requests;
using IdentityService.DTOs.Responses;
using IdentityService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetMe()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var response = await _userService.GetProfileAsync(userId.Value);
        return Ok(response);
    }

    [HttpPut("me")]
    public async Task<ActionResult<UserResponse>> UpdateMe([FromBody] UpdateProfileRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var response = await _userService.UpdateProfileAsync(userId.Value, request);
        return Ok(response);
    }

    private Guid? GetCurrentUserId()
    {
        var rawId = User.GetUserId();
        return Guid.TryParse(rawId, out var userId) ? userId : null;
    }
}
