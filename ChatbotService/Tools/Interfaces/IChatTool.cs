using System.Text.Json;

namespace ChatbotService.Tools.Interfaces;

public interface IChatTool
{
    string Name { get; }
    string Description { get; }
    object Parameters { get; }

    Task<string> ExecuteAsync(
        JsonElement arguments,
        Guid authenticatedUserId,
        string accessToken,
        CancellationToken cancellationToken = default
    );
}