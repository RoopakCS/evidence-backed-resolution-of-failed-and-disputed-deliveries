using ChatbotService.AI;
using ChatbotService.Configuration;
using ChatbotService.DTOs;
using ChatbotService.DTOs.Groq;
using ChatbotService.Enums;
using ChatbotService.Models;
using ChatbotService.Repositories.Interfaces;
using ChatbotService.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace ChatbotService.Services.Implementations;

public class ChatService : IChatService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly GroqClient _groqClient;
    private readonly PromptLoader _promptLoader;
    private readonly ChatbotSettings _chatbotSettings;
    private readonly GroqSettings _groqSettings;

    public ChatService(
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository,
        GroqClient groqClient,
        PromptLoader promptLoader,
        IOptions<ChatbotSettings> chatbotSettings,
        IOptions<GroqSettings> groqSettings)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _groqClient = groqClient;
        _promptLoader = promptLoader;
        _chatbotSettings = chatbotSettings.Value;
        _groqSettings = groqSettings.Value;
    }

    public async Task<SendMessageResponse> SendMessageAsync(
        Guid customerId,
        SendMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository
            .GetByCustomerIdAsync(customerId, cancellationToken);

        if (conversation == null)
        {
            conversation = new Conversation
            {
                CustomerId = customerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            conversation = await _conversationRepository
                .CreateAsync(conversation, cancellationToken);
        }

        var userMessage = new Message
        {
            ConversationId = conversation.Id,
            Sender = MessageSender.User,
            Content = request.Message,
            CreatedAt = DateTime.UtcNow
        };

        userMessage = await _messageRepository.CreateAsync(userMessage, cancellationToken);

        var history = await _messageRepository.GetByConversationIdAsync(
            conversation.Id,
            cancellationToken);

        if (!history.Any(message => message.Id == userMessage.Id))
        {
            history.Add(userMessage);
        }

        var maxConversationMessages = Math.Max(1, _chatbotSettings.MaxConversationMessages);
        var groqMessages = new List<GroqMessageDto>
        {
            new()
            {
                Role = "system",
                Content = await _promptLoader.LoadSystemPromptAsync(cancellationToken)
            }
        };

        groqMessages.AddRange(history
            .Where(message => message.Sender is MessageSender.User or MessageSender.Assistant)
            .OrderBy(message => message.CreatedAt)
            .ThenBy(message => message.Id)
            .TakeLast(maxConversationMessages)
            .Select(message => new GroqMessageDto
            {
                Role = message.Sender == MessageSender.User ? "user" : "assistant",
                Content = message.Content
            }));

        var groqResponse = await _groqClient.GetChatCompletionAsync(
            new GroqChatRequestDto
            {
                Model = _groqSettings.Model,
                Temperature = _groqSettings.Temperature,
                MaxCompletionTokens = _groqSettings.MaxTokens,
                Messages = groqMessages
            },
            cancellationToken);

        var assistantContent = groqResponse.Choices?
            .FirstOrDefault()?.Message?.Content;

        if (string.IsNullOrWhiteSpace(assistantContent))
        {
            throw new InvalidOperationException(
                "Groq did not return assistant text for the chat completion.");
        }

        var assistantMessage = new Message
        {
            ConversationId = conversation.Id,
            Sender = MessageSender.Assistant,
            Content = assistantContent,
            CreatedAt = DateTime.UtcNow
        };

        assistantMessage = await _messageRepository
            .CreateAsync(assistantMessage, cancellationToken);

        conversation.UpdatedAt = DateTime.UtcNow;

        await _conversationRepository.UpdateAsync(conversation, cancellationToken);

        return new SendMessageResponse
        {
            UserMessage = MapToMessageResponse(userMessage),
            AssistantMessage = MapToMessageResponse(assistantMessage)
        };
    }

    public async Task<ChatHistoryResponse> GetHistoryAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository
            .GetByCustomerIdWithMessagesAsync(customerId, cancellationToken);

        // Customer has no conversation yet
        if (conversation == null)
        {
            return new ChatHistoryResponse
            {
                ConversationId = Guid.Empty,
                Messages = []
            };
        }

        return new ChatHistoryResponse
        {
            ConversationId = conversation.Id,
            Messages = conversation.Messages
                .OrderBy(m => m.CreatedAt)
                .Select(MapToMessageResponse)
                .ToList()
        };
    }

    private static MessageResponse MapToMessageResponse(Message message)
    {
        return new MessageResponse
        {
            Id = message.Id,
            Sender = message.Sender,
            Content = message.Content,
            CreatedAt = message.CreatedAt,
            Citations = []
        };
    }
}