namespace server.DTO;

public class AuthResponseDto
{
    public bool success { get; set; }
    public string message { get; set; } = string.Empty;
    public Guid user_id { get; set; }
    public string firstname { get; set; } = string.Empty;
    public string lastname { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string? token { get; set; }
    public DateTime? token_expires_at { get; set; }
}