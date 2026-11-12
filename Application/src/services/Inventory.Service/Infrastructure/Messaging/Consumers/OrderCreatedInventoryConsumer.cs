using Inventory.Service.Common.Domain;
using Inventory.Service.Features.Commands;
using Inventory.Service.Infrastructure.Persistence;
using MassTransit;
using MediatR;
using MicroService.Contracts.Events;

namespace Inventory.Service.Infrastructure.Messaging.Consumers;

public class OrderCreatedInventoryConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly InventoryDbContext _context;

    public OrderCreatedInventoryConsumer(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;

        foreach (var item in message.Items)
        {
            _context.OrderItemReservation.Add(
                OrderItemReservation.Create(message.OrderId, item.ProductId, item.Quantity));
        }

        await _context.SaveChangesAsync(context.CancellationToken);

        Console.WriteLine($"[Inventory] Saved {message.Items.Count} items for OrderId={message.OrderId}");
    }
}