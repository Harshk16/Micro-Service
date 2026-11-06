
using Microsoft.EntityFrameworkCore;
using Product.Service.Features.ProductItems;

namespace Product.Service.Infrastructure.Persistence.Configurations;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }

    public DbSet<ProductItem> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Industry practice: Automatically scans this service project assembly 
        // and applies all implementation rules from IEntityTypeConfiguration (File 2)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductDbContext).Assembly);
    }
}