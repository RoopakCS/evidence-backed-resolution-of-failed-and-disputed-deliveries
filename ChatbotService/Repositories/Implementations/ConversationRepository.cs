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

    public async Task<Conversation?> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
            .FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);
    }

    public async Task<Conversation?> GetByCustomerIdWithMessagesAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);
    }

    public async Task<Conversation> CreateAsync(
        Conversation conversation,
        CancellationToken cancellationToken = default)
    {
        await _context.Conversations.AddAsync(conversation, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return conversation;
    }

    public async Task UpdateAsync(
        Conversation conversation,
        CancellationToken cancellationToken = default)
    {
        _context.Conversations.Update(conversation);

        await _context.SaveChangesAsync(cancellationToken);
    }
}