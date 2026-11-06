using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Service.Features.InventoryItems;

namespace Inventory.Service.Infrastructure.Persistence.Configurations;

// public class InventoryConfiguration : IEntityTypeConfiguration<InventoryItem>
// {
//     public void Configure(EntityTypeBuilder<InventoryItem> builder)
//     {
//         // 1. Explicitly name the database table in clean lowercase
//         builder.ToTable("inventory_items");

//         // 2. Primary Key setup
//         builder.HasKey(i => i.Id);

//         // 3. Natural business key constraints (SKU)
//         builder.Property(i => i.Sku)
//             .HasMaxLength(50)
//             .IsRequired();

//         // 4. Force unique index rule so a SKU can only exist once in inventory
//         builder.HasIndex(i => i.Sku)
//             .IsUnique();

//         // 5. Quantity validation configuration
//         builder.Property(i => i.QuantityAvailable)
//             .IsRequired();
            
//         builder.Property(i => i.UpdatedAtUtc)
//             .IsRequired(false);
//     }
// }

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("InventoryItems");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Sku).HasMaxLength(64).IsRequired();

        builder.HasIndex(x => x.ProductId).IsUnique();   // one row per product
        builder.HasIndex(x => x.Sku).IsUnique();

        builder.Property<uint>("xmin").IsRowVersion();   // Postgres concurrency token
    }
}
