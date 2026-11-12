using MassTransit;
using MediatR;
using MicroService.Contracts.Events;
using Order.Api.Features.Commands;
using Order.Service.Features.Commands;

namespace Order.Service.Infrastructure.Messaging.Consumers;

public class PaymentCompletedConsumer : IConsumer<PaymentCompletedEvent>
{
    private readonly IMediator _mediator;
    public PaymentCompletedConsumer(IMediator mediator)
    {
        _mediator = mediator;    
    }

    public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
    {
        Console.WriteLine($"[Order] PaymentCompleted received: OrderId={context.Message.OrderId} PaymentId={context.Message.PaymentId}");
        // return Task.CompletedTask;
        await _mediator.Send(new ConfirmOrderCommand(context.Message.OrderId));
    }
}