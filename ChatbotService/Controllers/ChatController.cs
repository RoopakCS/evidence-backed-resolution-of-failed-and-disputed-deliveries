using ChatbotService.DTOs;
using ChatbotService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace ChatbotService.Controllers;

[ApiController]
[Route("api/chat")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("messages")]
    public async Task<ActionResult<SendMessageResponse>> SendMessage(
        [FromBody] SendMessageRequest request,
        CancellationToken cancellationToken)
    {
        var customerId = GetCustomerId();

        if (customerId == null)
        {
            return Unauthorized();
        }

        if (!AuthenticationHeaderValue.TryParse(
                Request.Headers.Authorization.ToString(),
                out var authorization)
            || !string.Equals(
                authorization.Scheme,
                "Bearer",
                StringComparison.OrdinalIgnoreCase)
            || string.IsNullOrWhiteSpace(authorization.Parameter))
        {
            return Unauthorized();
        }

        var response = await _chatService.SendMessageAsync(
            customerId.Value,
            request,
            authorization.Parameter,
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("history")]
    public async Task<ActionResult<ChatHistoryResponse>> GetHistory(
        CancellationToken cancellationToken)
    {
        var customerId = GetCustomerId();

        if (customerId == null)
        {
            return Unauthorized();
        }

        var response = await _chatService.GetHistoryAsync(
            customerId.Value,
            cancellationToken);

        return Ok(response);
    }

    private Guid? GetCustomerId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (Guid.TryParse(userId, out var customerId))
        {
            return customerId;
        }

        return null;
    }
}