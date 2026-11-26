namespace ChatbotService.Configuration;

public class PromptLoader
{
    private readonly string _promptPath;

    public PromptLoader()
    {
        _promptPath = Path.Combine(
            AppContext.BaseDirectory,
            "Configuration",
            "Prompts",
            "SystemPrompt.txt");
    }

    public async Task<string> LoadSystemPromptAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_promptPath))
        {
            throw new FileNotFoundException(
                "System prompt file was not found.",
                _promptPath);
        }

        return await File.ReadAllTextAsync(_promptPath, cancellationToken);
    }
}