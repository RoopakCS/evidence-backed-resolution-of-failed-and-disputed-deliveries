using server.Data;
using server.Interfaces;
using server.Models;
using Microsoft.EntityFrameworkCore;

namespace server.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext appDb;

        public CustomerRepository(AppDbContext dbContext)
        {
            appDb = dbContext;
        }

        public async Task AddAsync(Customer customer)
        {
            await appDb.Customers.AddAsync(customer);
            await appDb.SaveChangesAsync();
        }

        public async Task UpdateAsync(Customer customer)
        {
            appDb.Customers.Update(customer);
            await appDb.SaveChangesAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await appDb.Customers.AnyAsync(c => c.Email == email);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await appDb.Customers.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Customer?> GetByIdAsync(Guid id)
        {
            return await appDb.Customers.FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}