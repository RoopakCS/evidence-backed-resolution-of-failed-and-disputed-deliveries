using ChatbotService.Enums;

namespace ChatbotService.DTOs;

public class MessageResponse
{
    public Guid Id { get; set; }

    public MessageSender Sender { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public IReadOnlyCollection<AssistantCitationDto> Citations { get; set; } = Array.Empty<AssistantCitationDto>();
}