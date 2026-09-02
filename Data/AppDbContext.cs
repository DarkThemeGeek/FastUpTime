using FastUpTime.Models;
using Microsoft.EntityFrameworkCore;

namespace FastUpTime.Data;

public class AppDbContext : DbContext
{
    
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<UserAccount> UserAccounts { get; set; }
    public DbSet<Site> Sites { get; set; }
    public DbSet<SitePing> SitePings { get; set; }
}