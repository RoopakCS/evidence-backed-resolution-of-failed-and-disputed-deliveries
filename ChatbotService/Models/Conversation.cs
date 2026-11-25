namespace ChatbotService.Models;

public class Conversation
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Navigation property
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}