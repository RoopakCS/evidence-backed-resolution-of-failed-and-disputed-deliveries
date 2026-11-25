using System.ComponentModel.DataAnnotations;

namespace ChatbotService.DTOs;

public class SendMessageRequest
{
    [Required]
    [RegularExpression(@"^[\s\S]*\S[\s\S]*$", ErrorMessage = "Message cannot be empty or whitespace.")]
    public string Message { get; set; } = string.Empty;
}