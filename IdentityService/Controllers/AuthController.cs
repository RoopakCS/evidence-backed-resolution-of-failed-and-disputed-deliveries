using IdentityService.DTOs.Requests;
using IdentityService.DTOs.Responses;
using IdentityService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request, GetDeviceMetadata());
        return Created(string.Empty, response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request, GetDeviceMetadata());
        return Ok(response);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<RefreshResponse>> Refresh([FromBody] RefreshRequest request)
    {
        var response = await _authService.RefreshAsync(request, GetDeviceMetadata());
        return Ok(response);
    }

    [HttpPost("revoke")]
    [Authorize]
    public async Task<IActionResult> Revoke([FromBody] RevokeRefreshTokenRequest request)
    {
        await _authService.RevokeAsync(request);
        return NoContent();
    }

    private string? GetDeviceMetadata()
    {
        var userAgent = Request.Headers.UserAgent.ToString();
        if (string.IsNullOrWhiteSpace(userAgent))
            return null;

        return userAgent.Length > 500 ? userAgent[..500] : userAgent;
    }
}
