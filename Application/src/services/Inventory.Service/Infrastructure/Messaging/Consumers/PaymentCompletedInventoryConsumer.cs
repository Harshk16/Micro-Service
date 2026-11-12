using MassTransit;
using MediatR;
using MicroService.Contracts.Events;
using Inventory.Service.Features.Commands;
using Inventory.Service.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Service.Infrastructure.Messaging.Consumers;

public class PaymentCompletedInventoryConsumer : IConsumer<PaymentCompletedEvent>
{
    private readonly InventoryDbContext _context;
    private readonly IMediator _mediator;

    public PaymentCompletedInventoryConsumer(InventoryDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
    {
        var orderId = context.Message.OrderId;

        var items = await _context.OrderItemReservation
            .Where(x => x.OrderId == orderId && !x.IsReserved)
            .ToListAsync(context.CancellationToken);

        foreach (var item in items)
        {
            await _mediator.Send(new ReserveInventoryCommand(item.ProductId, item.Quantity), context.CancellationToken);
            item.MarkReserved();
        }

        await _context.SaveChangesAsync(context.CancellationToken);

        Console.WriteLine($"[Inventory] Reserved {items.Count} items for OrderId={orderId}");
    }
}