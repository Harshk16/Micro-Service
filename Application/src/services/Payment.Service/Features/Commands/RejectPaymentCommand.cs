using MassTransit;
using MediatR;
using MicroService.Contracts.Events;
using Microsoft.EntityFrameworkCore;
using Payment.Service.Infrastructure.Persistence.Configurations;

namespace Payment.Service.Features.Commands;

public record RejectPaymentCommand(Guid OrderId, string Reason) : IRequest<RejectPaymentResponse>;

public record RejectPaymentResponse(Guid PaymentId, Guid OrderId, string Status, string? FailureReason, DateTime? UpdatedAtUtc);

public class RejectPaymentHandler : IRequestHandler<RejectPaymentCommand, RejectPaymentResponse>
{
    private readonly PaymentDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public RejectPaymentHandler(PaymentDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<RejectPaymentResponse> Handle(RejectPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(x => x.OrderId == request.OrderId, cancellationToken);

        if (payment is null)
            throw new KeyNotFoundException($"Payment for order {request.OrderId} not found.");

        payment.Reject(request.Reason);   // entity allows this only from Pending, and needs a reason
        await _context.SaveChangesAsync(cancellationToken);
        
         await _publishEndpoint.Publish(
            new PaymentFailedEvent(payment.OrderId, request.Reason), cancellationToken);

        return new RejectPaymentResponse(payment.Id, payment.OrderId, payment.Status.ToString(), payment.FailureReason, payment.UpdatedAtUtc);
    }
}