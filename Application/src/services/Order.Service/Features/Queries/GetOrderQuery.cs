using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Service.Infrastructure.Persistence.Configurations;

namespace Order.Api.Features.Queries;

public record GetOrderQuery(Guid OrderId) : IRequest<GetOrderResponse>;

public record GetOrderItemResponse(Guid ProductId, string Sku, int Quantity, decimal UnitPrice, decimal LineTotal);

public record GetOrderResponse(
    Guid OrderId,
    Guid CustomerId,
    string Currency,
    decimal TotalAmount,
    string Status,
    string? CancellationReason,
    List<GetOrderItemResponse> Items,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public class GetOrderHandler : IRequestHandler<GetOrderQuery, GetOrderResponse>
{
    private readonly OrderDbContext _context;

    public GetOrderHandler(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<GetOrderResponse> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null)
            throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");

        return new GetOrderResponse(
            order.Id,
            order.CustomerId,
            order.Currency,
            order.TotalAmount,
            order.Status.ToString(),
            order.CancellationReason,
            order.Items
                .Select(i => new GetOrderItemResponse(i.ProductId, i.Sku, i.Quantity, i.UnitPrice, i.LineTotal))
                .ToList(),
            order.CreatedAtUtc,
            order.UpdatedAtUtc);
    }
}