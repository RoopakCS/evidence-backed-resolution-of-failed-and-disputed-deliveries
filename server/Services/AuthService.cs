using server.DTO;
using server.Models;
using server.Interfaces;

namespace server.Services;

public class AuthService : IAuthService
{
    private readonly ICustomerRepository _customerRepository;

    public AuthService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        string email = dto.email.Trim().ToLowerInvariant();

        bool emailExists =
            await _customerRepository.EmailExistsAsync(email);

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

        await _customerRepository.AddAsync(customer);

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


    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        string email = dto.email.Trim().ToLowerInvariant();

        var customer =
            await _customerRepository.GetByEmailAsync(email);

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

        return new AuthResponseDto
        {
            success = true,
            message = "Login successful",
            user_id = customer.Id,
            firstname = customer.FirstName,
            lastname = customer.LastName,
            email = customer.Email
        };
    }
}