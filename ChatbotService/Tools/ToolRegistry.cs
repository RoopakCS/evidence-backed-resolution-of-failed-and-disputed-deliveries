using ChatbotService.Tools.Interfaces;
using ChatbotService.Tools.DTOs;

namespace ChatbotService.Tools;

public class ToolRegistry
{
    private readonly Dictionary<string, IChatTool> _tools;

    public ToolRegistry(IEnumerable<IChatTool> tools)
    {
        _tools = tools.ToDictionary(
            tool => tool.Name,
            StringComparer.OrdinalIgnoreCase
        );
    }

    public IChatTool? GetTool(string name)
    {
        _tools.TryGetValue(name, out var tool);

        return tool;
    }

    public IReadOnlyCollection<IChatTool> GetAllTools()
    {
        return _tools.Values;
    }

    public List<ChatToolDefinition> GetToolDefinitions()
    {
        return _tools.Values
            .Select(tool => new ChatToolDefinition
            {
                Type = "function",
                Function = new ChatToolFunctionDefinition
                {
                    Name = tool.Name,
                    Description = tool.Description,
                    Parameters = tool.Parameters
                }
            })
            .ToList();
    }
}