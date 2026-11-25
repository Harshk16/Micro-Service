using MassTransit;
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


        // MassTransit outbox and inbox tables.
        // Needed so the EF Core migration creates them.
        // Without these, the outbox has nowhere to save events.
        modelBuilder.AddInboxStateEntity();     // inbox: duplicate-message tracking for consumers
        modelBuilder.AddOutboxMessageEntity();  // outbox: messages waiting to be sent to RabbitMQ
        modelBuilder.AddOutboxStateEntity();    // outbox: delivery progress for the sender
    }
}