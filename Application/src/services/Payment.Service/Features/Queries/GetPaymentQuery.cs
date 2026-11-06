using MediatR;
using Payment.Service.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Payment.Service.Features.Queries;

public record GetPaymentQuery(Guid OrderId) : IRequest<GetPaymentResponse>;

public record GetPaymentResponse(Guid PaymentId, Guid OrderId, Guid CustomerId, decimal Amount, string Currency, string Status, string? FailureReason, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc);

public class GetPaymentHandler : IRequestHandler<GetPaymentQuery, GetPaymentResponse>
{
    private readonly PaymentDbContext _context;

    public GetPaymentHandler(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<GetPaymentResponse> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(x => x.OrderId == request.OrderId, cancellationToken);

        if (payment is null)
            throw new KeyNotFoundException($"Payment for order {request.OrderId} not found.");

        return new GetPaymentResponse(
            payment.Id,
            payment.OrderId,
            payment.CustomerId,
            payment.Amount,
            payment.Currency,
            payment.Status.ToString(),
            payment.FailureReason,
            payment.CreatedAtUtc,
            payment.UpdatedAtUtc);
    }
}