using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using server.Data;
using server.DTO;
using server.Models;

namespace server.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly int _jwtExpiryMinutes;

    public AuthService(AppDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _jwtSecret = configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("Jwt:Secret is not configured.");
        _jwtIssuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
        _jwtAudience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

        if (!int.TryParse(configuration["Jwt:ExpiryMinutes"], out _jwtExpiryMinutes)
            || _jwtExpiryMinutes <= 0)
        {
            throw new InvalidOperationException("Jwt:ExpiryMinutes must be a positive integer.");
        }
    }

    public async Task<AuthResponseDto> RegisterAsync(
        RegisterDto dto,
        CancellationToken cancellationToken)
    {
        string email = dto.email.Trim().ToLowerInvariant();

        bool emailExists = await _dbContext.Customers
            .AnyAsync(customer => customer.Email == email, cancellationToken);

        if (emailExists)
        {
            return new AuthResponseDto
            {
                success = false,
                message = "Email already exists"
            };
        }

        string hashedPassword =
            BCrypt.Net.BCrypt.HashPassword(dto.password);

        var customer = new Customer
        {
            FirstName = dto.firstname.Trim(),
            LastName = dto.lastname.Trim(),
            Email = email,
            PhoneNumber = dto.phonenumber.Trim(),
            PasswordHash = hashedPassword
        };

        await _dbContext.Customers.AddAsync(customer, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto
        {
            success = true,
            message = "Registration successful",
            user_id = customer.Id,
            firstname = customer.FirstName,
            lastname = customer.LastName,
            email = customer.Email
        };
    }

    public async Task<AuthResponseDto> LoginAsync(
        LoginDto dto,
        CancellationToken cancellationToken)
    {
        string email = dto.email.Trim().ToLowerInvariant();

        var customer = await _dbContext.Customers
            .FirstOrDefaultAsync(existingCustomer => existingCustomer.Email == email, cancellationToken);

        if (customer == null)
        {
            return new AuthResponseDto
            {
                success = false,
                message = "Invalid email or password"
            };
        }

        bool isPasswordValid =
            BCrypt.Net.BCrypt.Verify(
                dto.password,
                customer.PasswordHash
            );

        if (!isPasswordValid)
        {
            return new AuthResponseDto
            {
                success = false,
                message = "Invalid email or password"
            };
        }

        DateTime expiresAt = DateTime.UtcNow.AddMinutes(_jwtExpiryMinutes);

        return new AuthResponseDto
        {
            success = true,
            message = "Login successful",
            user_id = customer.Id,
            firstname = customer.FirstName,
            lastname = customer.LastName,
            email = customer.Email,
            token = GenerateToken(customer, expiresAt),
            token_expires_at = expiresAt
        };
    }

    public async Task<UserDto?> GetUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Customers
            .Where(customer => customer.Id == userId)
            .Select(customer => new UserDto
            {
                user_id = customer.Id,
                firstname = customer.FirstName,
                lastname = customer.LastName,
                email = customer.Email
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    private string GenerateToken(Customer customer, DateTime expiresAt)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new Claim(ClaimTypes.Name, $"{customer.FirstName} {customer.LastName}"),
            new Claim(ClaimTypes.Email, customer.Email),
            new Claim(JwtRegisteredClaimNames.Email, customer.Email)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}