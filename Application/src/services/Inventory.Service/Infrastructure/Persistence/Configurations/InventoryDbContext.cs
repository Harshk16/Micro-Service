using Microsoft.EntityFrameworkCore;
using Inventory.Service.Features.InventoryItems;
using Inventory.Service.Common.Domain;
using MassTransit;

namespace Inventory.Service.Infrastructure.Persistence;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<OrderItemReservation> OrderItemReservation => Set<OrderItemReservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Industry practice: Automatically scans this service project assembly
        // and applies all implementation rules from IEntityTypeConfiguration (File 2)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);

        modelBuilder.AddInboxStateEntity();     // inbox: duplicate-message tracking for consumers
        modelBuilder.AddOutboxMessageEntity();  // outbox: messages waiting to be sent to RabbitMQ
        modelBuilder.AddOutboxStateEntity();    // outbox: delivery progress for the sender
    }
}
