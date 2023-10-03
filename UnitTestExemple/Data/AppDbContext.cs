using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using UnitTestExemple.Domain.Entities;

namespace UnitTestExemple.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public AppDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Order>()
            .HasMany(e => e.Items)
            .WithOne()
            .IsRequired();

        modelBuilder
            .Entity<Order>()
            .Navigation(e => e.Items)
            .AutoInclude();
    }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await this.Database.BeginTransactionAsync();
    }
}