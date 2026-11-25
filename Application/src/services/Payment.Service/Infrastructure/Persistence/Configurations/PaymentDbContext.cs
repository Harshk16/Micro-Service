using MassTransit;
using Microsoft.EntityFrameworkCore;
using Payment.Service.Features;

namespace Payment.Service.Infrastructure.Persistence.Configurations;


public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }

    public DbSet<PaymentItem> Payments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PaymentItemConfiguration());

        modelBuilder.AddInboxStateEntity();     // inbox: duplicate-message tracking for consumers
        modelBuilder.AddOutboxMessageEntity();  // outbox: messages waiting to be sent  to RabbitMQ
        modelBuilder.AddOutboxStateEntity();    // outbox: delivery progress for the sender
    }
}