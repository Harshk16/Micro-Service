using Microsoft.EntityFrameworkCore;
using Order.Api.Domain.Entities;

namespace Order.Service.Infrastructure.Persistence.Configurations;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }
    public DbSet<Orders> Orders => Set<Orders>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Industry practice: Automatically scans this service project assembly 
        // and applies all implementation rules from IEntityTypeConfiguration (File 2)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
    }
}