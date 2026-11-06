using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Api.Domain.Entities;

namespace Order.Api.Persistence.Configurations;

public class OrdersConfiguration : IEntityTypeConfiguration<Orders>
{
    public void Configure(EntityTypeBuilder<Orders> builder)
    {
        builder.ToTable("orders");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Currency)
            .HasMaxLength(3)
            .IsRequired();

        // stored as text ("Pending", "Confirmed", "Cancelled")
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.CancellationReason).HasMaxLength(500);
        builder.Property(x => x.CreatedAtUtc).IsRequired();

        // one order has many items, items are deleted with the order
        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // EF fills the private _items list
        builder.Navigation(x => x.Items).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => x.CustomerId);
    }
}