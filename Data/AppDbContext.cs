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
    public DbSet<UserAccountSite> UserAccountSites { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccountSite>()
            .HasKey(x => new { x.UserAccountId, x.SiteId });

        modelBuilder.Entity<UserAccountSite>()
            .HasOne(x => x.UserAccount)
            .WithMany(x=>x.Sites)
            .HasForeignKey(x => x.UserAccountId);

        modelBuilder.Entity<UserAccountSite>()
            .HasOne(x => x.Site)
            .WithMany(x=>x.Accounts)
            .HasForeignKey(x => x.SiteId);
    }
}