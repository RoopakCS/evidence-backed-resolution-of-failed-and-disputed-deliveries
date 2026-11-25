using ChatbotService.Data;
using ChatbotService.Models;
using ChatbotService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatbotService.Repositories.Implementations;

public class ConversationRepository : IConversationRepository
{
    private readonly ChatbotDbContext _context;

    public ConversationRepository(ChatbotDbContext context)
    {
        _context = context;
    }

    public async Task<Conversation?> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.Conversations
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);
    }

    public async Task<Conversation?> GetByCustomerIdWithMessagesAsync(Guid customerId)
    {
        return await _context.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);
    }

    public async Task<Conversation> CreateAsync(Conversation conversation)
    {
        await _context.Conversations.AddAsync(conversation);

        await _context.SaveChangesAsync();

        return conversation;
    }

    public async Task UpdateAsync(Conversation conversation)
    {
        _context.Conversations.Update(conversation);

        await _context.SaveChangesAsync();
    }
}