using ChatbotService.DTOs;

namespace ChatbotService.Services.Interfaces;

public interface IChatService
{
    Task<SendMessageResponse> SendMessageAsync(
        Guid customerId,
        SendMessageRequest request,
        string accessToken,
        CancellationToken cancellationToken = default);

    Task<ChatHistoryResponse> GetHistoryAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);
}