using Microsoft.EntityFrameworkCore;
using server;

namespace server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // public DbSet<Student> students {get;set;}
    // public DbSet<User> users {get;set;}
}