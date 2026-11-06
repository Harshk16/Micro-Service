using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Service.Infrastructure.Persistence.Configurations;

namespace Order.Api.Features.Commands;

public record ConfirmOrderCommand(Guid OrderId) : IRequest<ConfirmOrderResponse>;

public record ConfirmOrderResponse(Guid OrderId, string Status, DateTime? UpdatedAtUtc);

public class ConfirmOrderHandler : IRequestHandler<ConfirmOrderCommand, ConfirmOrderResponse>
{
    private readonly OrderDbContext _context;

    public ConfirmOrderHandler(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<ConfirmOrderResponse> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        // Include items: Confirm() checks that the order has at least one item
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null)
            throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");

        order.Confirm();   // entity allows this only from Pending
        await _context.SaveChangesAsync(cancellationToken);

        return new ConfirmOrderResponse(order.Id, order.Status.ToString(), order.UpdatedAtUtc);
    }
}