using MassTransit;
using MediatR;
using MicroService.Contracts.Events;
using Order.Api.Domain.Entities;
using Order.Service.Infrastructure.Persistence.Configurations;

namespace Order.Service.Features.Commands;

public record OrderItemRequest(Guid ProductId, string Sku, int Quantity, decimal UnitPrice);

public record CreateOrderCommand(Guid CustomerId, string Currency, List<OrderItemRequest> Items) : IRequest<CreateOrderResponse>;

public record OrderItemResponse(Guid ProductId, string Sku, int Quantity, decimal UnitPrice, decimal LineTotal);

public record CreateOrderResponse(
    Guid OrderId,
    Guid CustomerId,
    string Currency,
    decimal TotalAmount,
    string Status,
    List<OrderItemResponse> Items,
    DateTime CreatedAtUtc);

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    private readonly OrderDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;


    public CreateOrderHandler(OrderDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.Items is null || request.Items.Count == 0)
            throw new ArgumentException("An order needs at least one item.");

        var order = Orders.Create(Guid.NewGuid(), request.CustomerId, request.Currency);

        foreach (var item in request.Items)
        {
            order.AddItem(item.ProductId, item.Sku, item.Quantity, item.UnitPrice);
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        // The broker comes later: this is where OrderSubmittedEvent will be published.

        // await _publishEndpoint.Publish(
        //     new OrderCreatedEvent(Guid.NewGuid(), order.CustomerId, order.TotalAmount, order.Currency), cancellationToken);

        var evt = new OrderCreatedEvent(
            order.Id,
            order.CustomerId,
            order.TotalAmount,
            order.Currency,
            order.Items.Select(i => new OrderItemContract(i.ProductId, i.Sku, i.Quantity)).ToList());

        await _publishEndpoint.Publish(evt, cancellationToken);

        return new CreateOrderResponse(
            order.Id,
            order.CustomerId,
            order.Currency,
            order.TotalAmount,
            order.Status.ToString(),
            order.Items
                .Select(i => new OrderItemResponse(i.ProductId, i.Sku, i.Quantity, i.UnitPrice, i.LineTotal))
                .ToList(),
            order.CreatedAtUtc);
    }
}

