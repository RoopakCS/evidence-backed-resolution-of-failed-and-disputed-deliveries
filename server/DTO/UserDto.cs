namespace server.DTO;

public class UserDto
{
    public Guid user_id { get; set; }
    public string firstname { get; set; } = string.Empty;
    public string lastname { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
}
