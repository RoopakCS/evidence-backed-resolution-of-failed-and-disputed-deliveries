using ChatbotService.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatbotService.Data;

public class ChatbotDbContext : DbContext
{
    public ChatbotDbContext(DbContextOptions<ChatbotDbContext> options)
        : base(options)
    {
    }

    public DbSet<Conversation> Conversations => Set<Conversation>();

    public DbSet<Message> Messages => Set<Message>();

    public DbSet<ToolCall> ToolCalls => Set<ToolCall>();
}