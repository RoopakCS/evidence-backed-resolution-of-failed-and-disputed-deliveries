using server.DTO;

namespace server.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken);

    Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken);

    Task<UserDto?> GetUserAsync(Guid userId, CancellationToken cancellationToken);
}