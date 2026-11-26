using System.Text.Json;

namespace ChatbotService.Tools;

public class ToolExecutor
{
    private readonly ToolRegistry _toolRegistry;

    public ToolExecutor(ToolRegistry toolRegistry)
    {
        _toolRegistry = toolRegistry;
    }

    public async Task<string> ExecuteAsync(
        string toolName,
        JsonElement arguments,
        Guid authenticatedUserId,
        string accessToken,
        CancellationToken cancellationToken = default
    )
    {
        var tool = _toolRegistry.GetTool(toolName);

        if (tool is null)
        {
            throw new InvalidOperationException(
                $"Tool '{toolName}' is not registered."
            );
        }

        return await tool.ExecuteAsync(
            arguments,
            authenticatedUserId,
            accessToken,
            cancellationToken
        );
    }
}