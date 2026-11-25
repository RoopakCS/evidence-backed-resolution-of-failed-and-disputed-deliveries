using ChatbotService.Models;

namespace ChatbotService.Repositories.Interfaces;

public interface IConversationRepository
{
    Task<Conversation?> GetByCustomerIdAsync(Guid customerId);

    Task<Conversation?> GetByCustomerIdWithMessagesAsync(Guid customerId);

    Task<Conversation> CreateAsync(Conversation conversation);

    Task UpdateAsync(Conversation conversation);
}