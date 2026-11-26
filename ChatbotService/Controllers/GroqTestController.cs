using ChatbotService.AI;
using ChatbotService.DTOs.Groq;
using Microsoft.AspNetCore.Mvc;

namespace ChatbotService.Controllers;

[ApiController]
[Route("api/test/groq")]
public class GroqTestController : ControllerBase
{
    private readonly GroqClient _groqClient;

    public GroqTestController(GroqClient groqClient)
    {
        _groqClient = groqClient;
    }

    [HttpGet]
    public async Task<IActionResult> TestGroq(
        CancellationToken cancellationToken)
    {
        var request = new GroqChatRequestDto
        {
            Model = "openai/gpt-oss-20b",
            Messages =
            [
                new GroqMessageDto
                {
                    Role = "user",
                    Content = "Reply with exactly: Groq connection successful!"
                }
            ],
            Temperature = 0.0,
            MaxCompletionTokens = 50
        };

        var response = await _groqClient.GetChatCompletionAsync(
            request,
            cancellationToken);

        var message = response.Choices[0].Message;

        return Ok(new
        {
            Response = message.Content,
            FinishReason = response.Choices[0].FinishReason,
            Usage = response.Usage
        });
    }
}