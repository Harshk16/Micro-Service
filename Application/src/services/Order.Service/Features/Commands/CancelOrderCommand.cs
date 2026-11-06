using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Service.Infrastructure.Persistence.Configurations;

namespace Order.Api.Features.Commands;

public record CancelOrderCommand(Guid OrderId, string Reason) : IRequest<CancelOrderResponse>;

public record CancelOrderResponse(Guid OrderId, string Status, string? CancellationReason, DateTime? UpdatedAtUtc);

public class CancelOrderHandler : IRequestHandler<CancelOrderCommand, CancelOrderResponse>
{
    private readonly OrderDbContext _context;

    public CancelOrderHandler(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<CancelOrderResponse> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null)
            throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");

        order.Cancel(request.Reason);   // entity allows this only from Pending, and needs a reason
        await _context.SaveChangesAsync(cancellationToken);

        return new CancelOrderResponse(order.Id, order.Status.ToString(), order.CancellationReason, order.UpdatedAtUtc);
    }
}