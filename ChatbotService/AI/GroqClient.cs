using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ChatbotService.Configuration;
using ChatbotService.DTOs.Groq;
using Microsoft.Extensions.Options;

namespace ChatbotService.AI;

public class GroqClient
{
    private readonly HttpClient _httpClient;
    private readonly GroqSettings _settings;

    public GroqClient(
        HttpClient httpClient,
        IOptions<GroqSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<GroqChatResponseDto> GetChatCompletionAsync(
        GroqChatRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(_settings.BaseUrl))
        {
            throw new InvalidOperationException("The Groq BaseUrl is not configured.");
        }

        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            throw new InvalidOperationException("The Groq API key is not configured.");
        }

        var endpoint = $"{_settings.BaseUrl.TrimEnd('/')}/chat/completions";
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);

        var json = JsonSerializer.Serialize(request);
        httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await _httpClient.SendAsync(
            httpRequest,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var safeDetails = string.IsNullOrWhiteSpace(responseBody)
                ? string.Empty
                : $" Response: {responseBody.Replace(_settings.ApiKey, "[REDACTED]", StringComparison.Ordinal)}";

            if (safeDetails.Length > 2048)
            {
                safeDetails = $"{safeDetails[..2048]}…";
            }

            throw new HttpRequestException(
                $"Groq returned HTTP {(int)response.StatusCode} ({response.StatusCode}).{safeDetails}",
                null,
                response.StatusCode);
        }

        if (string.IsNullOrWhiteSpace(responseBody))
        {
            throw new InvalidOperationException("Groq returned an empty response.");
        }

        var chatResponse = JsonSerializer.Deserialize<GroqChatResponseDto>(responseBody)
            ?? throw new JsonException("Groq returned a JSON null response instead of a chat completion.");

        if (chatResponse.Choices is null || chatResponse.Choices.Count == 0)
        {
            throw new JsonException("Groq returned a response without any chat completion choices.");
        }

        return chatResponse;
    }
}