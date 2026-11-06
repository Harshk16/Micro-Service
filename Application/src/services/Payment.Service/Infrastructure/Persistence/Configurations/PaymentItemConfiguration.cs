using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.Service.Features;

namespace Payment.Service.Infrastructure.Persistence.Configurations;

public class PaymentItemConfiguration : IEntityTypeConfiguration<PaymentItem>
{
    public void Configure(EntityTypeBuilder<PaymentItem> builder)
    {
        builder.ToTable("payments");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.Currency)
            .HasMaxLength(3)
            .IsRequired();

        // stored as text ("Pending", "Approved", "Rejected"), readable in the database
        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.FailureReason)
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(p => p.OrderId)
            .IsUnique();   // one payment per order
    }
}