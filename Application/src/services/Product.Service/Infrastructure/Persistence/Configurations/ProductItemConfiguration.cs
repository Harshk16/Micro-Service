
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Product.Service.Features.ProductItems;

namespace Product.Service.Infrastructure.Persistence.Configurations;

public class ProductItemConfiguration : IEntityTypeConfiguration<ProductItem>
{
    public void Configure(EntityTypeBuilder<ProductItem> builder)
    {
        // Explicitly map to a lowercase singular/plural database table name
        builder.ToTable("products");

        // Primary Key configuration
        builder.HasKey(p => p.Id);
        
        // SKU properties and constraints
        builder.Property(p => p.Sku)
            .HasMaxLength(50)
            .IsRequired();

        // Create a unique database index for lightning-fast SKU lookups across services
        builder.HasIndex(p => p.Sku)
            .IsUnique();

        builder.Property(p => p.Name)
            .HasMaxLength(255)
            .IsRequired();

        // Enforce safe numeric scale and precision for financial values
        builder.Property(p => p.PriceAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.PriceCurrency)
            .HasMaxLength(3)
            .HasDefaultValue("USD")
            .IsRequired();
            
        builder.Property(p => p.CreatedAtUtc)
            .IsRequired();
    }
}
