namespace ChatbotService.DTOs;

public class AssistantCitationDto
{
    public string SourceType { get; set; } = string.Empty;

    public Guid SourceId { get; set; }

    public string DisplayText { get; set; } = string.Empty;
}