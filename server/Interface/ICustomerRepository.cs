using server.Models;

namespace server.Interfaces
{
    public interface ICustomerRepository
    {
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task<bool> EmailExistsAsync(string email);
        Task<Customer?> GetByEmailAsync(string email);
        Task<Customer?> GetByIdAsync(Guid id);
    }
}