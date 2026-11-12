using MassTransit;
using MediatR;
using MicroService.Contracts.Events;
using Payment.Service.Features.Commands;

namespace Payment.Service.Infrastructure.Messaging.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IMediator _mediator;
    public OrderCreatedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;
        Console.WriteLine($"[Payment] OrderCreated received: OrderId={message.OrderId}, Amount={message.TotalAmount}, Currency={message.Currency}");

        // if (message.OrderId == Guid.Parse("00000000-0000-0000-0000-000000000001"))
        //     throw new Exception("RETRY TEST");

        await _mediator.Send(new CreatePaymentCommand(message.OrderId, message.CustomerId, message.TotalAmount, message.Currency));

    }
}