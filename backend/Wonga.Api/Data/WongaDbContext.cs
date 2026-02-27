using Microsoft.EntityFrameworkCore;
using Wonga.Api.Models;

namespace Wonga.Api.Data;

public class WongaDbContext : DbContext
{
    public WongaDbContext(DbContextOptions<WongaDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}