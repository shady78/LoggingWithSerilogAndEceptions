using LoggingWithSerilog.Models;
using Microsoft.EntityFrameworkCore;

namespace LoggingWithSerilog.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public virtual DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>()
         .Property(e => e.Id)
         .HasDefaultValueSql("gen_random_uuid()");
    }
}
