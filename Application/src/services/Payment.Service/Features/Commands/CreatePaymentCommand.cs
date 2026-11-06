using MediatR;
using Payment.Service.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Payment.Service.Features.Commands;

public record CreatePaymentCommand(Guid OrderId, Guid CustomerId, decimal Amount, string Currency) : IRequest<CreatePaymentResponse>;

public record CreatePaymentResponse(Guid PaymentId, Guid OrderId, Guid CustomerId, decimal Amount, string Currency, string Status, string? FailureReason, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc);

public class CreatePaymentHandler : IRequestHandler<CreatePaymentCommand, CreatePaymentResponse>
{
    private readonly PaymentDbContext _context;

    public CreatePaymentHandler(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<CreatePaymentResponse> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.Payments
            .AnyAsync(x => x.OrderId == request.OrderId, cancellationToken);

        if (exists)
            throw new InvalidOperationException($"A payment for order {request.OrderId} already exists.");

        var paymentItem = PaymentItem.Create(Guid.NewGuid(), request.OrderId, request.CustomerId, request.Amount, request.Currency);

        _context.Payments.Add(paymentItem);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreatePaymentResponse(
            paymentItem.Id,
            paymentItem.OrderId,
            paymentItem.CustomerId,
            paymentItem.Amount,
            paymentItem.Currency,
            paymentItem.Status.ToString(),
            paymentItem.FailureReason,
            paymentItem.CreatedAtUtc,
            paymentItem.UpdatedAtUtc);
    }
}