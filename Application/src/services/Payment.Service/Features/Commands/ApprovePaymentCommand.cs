using MassTransit;
using MediatR;
using MicroService.Contracts.Events;
using Microsoft.EntityFrameworkCore;
using Payment.Service.Infrastructure.Persistence.Configurations;
// keep your existing using for PaymentDbContext

namespace Payment.Service.Features.Commands;

public record ApprovePaymentCommand(Guid OrderId) : IRequest<ApprovePaymentResponse>;

public record ApprovePaymentResponse(Guid PaymentId, Guid OrderId, string Status, DateTime? UpdatedAtUtc);

public class ApprovePaymentHandler : IRequestHandler<ApprovePaymentCommand, ApprovePaymentResponse>
{
    private readonly PaymentDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public ApprovePaymentHandler(PaymentDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }   

    public async Task<ApprovePaymentResponse> Handle(ApprovePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(x => x.OrderId == request.OrderId, cancellationToken);

        if (payment is null)
            throw new KeyNotFoundException($"Payment for order {request.OrderId} not found.");

        payment.Approve();
        await _context.SaveChangesAsync(cancellationToken);

         await _publishEndpoint.Publish(
            new PaymentCompletedEvent(payment.OrderId, payment.Id), cancellationToken);

        return new ApprovePaymentResponse(payment.Id, payment.OrderId, payment.Status.ToString(), payment.UpdatedAtUtc);
    }
}