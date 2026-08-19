using FastUpTime.Models;
using Microsoft.EntityFrameworkCore;

namespace FastUpTime.Data;

public class UserDbContext : DbContext
{
    
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<UserAccount> UserAccounts { get; set; }
}