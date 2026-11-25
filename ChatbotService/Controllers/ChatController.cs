using ChatbotService.DTOs;
using ChatbotService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        [FromBody] SendMessageRequest request)
    {
        var customerId = GetCustomerId();

        if (customerId == null)
        {
            return Unauthorized();
        }

        var response = await _chatService.SendMessageAsync(
            customerId.Value,
            request);

        return Ok(response);
    }

    [HttpGet("history")]
    public async Task<ActionResult<ChatHistoryResponse>> GetHistory()
    {
        var customerId = GetCustomerId();

        if (customerId == null)
        {
            return Unauthorized();
        }

        var response = await _chatService.GetHistoryAsync(
            customerId.Value);

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