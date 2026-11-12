using MassTransit;
using MediatR;
using MicroService.Contracts.Events;
using Order.Api.Features.Commands;

namespace Order.Service.Infrastructure.Messaging.Consumers;

public class PaymentFailedConsumer : IConsumer<PaymentFailedEvent>
{
    private readonly IMediator _mediator;
    public PaymentFailedConsumer(IMediator mediator)
    {
        _mediator = mediator;    
    }

    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        Console.WriteLine($"[Order] PaymentFailed received: OrderId={context.Message.OrderId}");
        await _mediator.Send(new CancelOrderCommand(context.Message.OrderId, context.Message.Reason));
    }
}