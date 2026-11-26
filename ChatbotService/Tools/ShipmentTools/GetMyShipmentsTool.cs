using System.Text.Json;
using System.Net.Http.Headers;
using ChatbotService.Tools.Interfaces;

namespace ChatbotService.Tools.ShipmentTools;

public class GetMyShipmentsTool : IChatTool
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GetMyShipmentsTool(
        HttpClient httpClient,
        IConfiguration configuration
    )
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public string Name => "get_my_shipments";

    public string Description =>
        "Retrieves the authenticated user's shipments.";

    public object Parameters => new Dictionary<string, object>
    {
        ["type"] = "object",
        ["properties"] = new Dictionary<string, object>(),
        ["required"] = Array.Empty<string>(),
        ["additionalProperties"] = false
    };

    public async Task<string> ExecuteAsync(
        JsonElement arguments,
        Guid authenticatedUserId,
        string accessToken,
        CancellationToken cancellationToken = default
    )
    {
        var baseUrl = _configuration["ShipmentService:BaseUrl"];

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException(
                "Shipment Service BaseUrl is not configured."
            );
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"{baseUrl.TrimEnd('/')}/api/shipments?page=1&pageSize=10"
        );

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await _httpClient.SendAsync(
            request,
            cancellationToken
        );

        var responseContent = await response.Content.ReadAsStringAsync(
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Shipment Service returned {(int)response.StatusCode}: {responseContent}"
            );
        }

        return responseContent;
    }
}