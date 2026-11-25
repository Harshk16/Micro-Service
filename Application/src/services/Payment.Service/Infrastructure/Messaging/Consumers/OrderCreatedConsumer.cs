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

        // throw new Exception("FAULT TEST");

        // if (message.OrderId == Guid.Parse("00000000-0000-0000-0000-000000000001"))
        //     throw new Exception("RETRY TEST");

        // Console.WriteLine("CRASH TEST: stopping process now");
        // Environment.Exit(1);

        // if (message.OrderId == Guid.Parse("00000000-0000-0000-0000-000000000004") && DateTime.UtcNow.Second % 2 == 0)
        //     throw new Exception("TRANSIENT TEST");

        await _mediator.Send(new CreatePaymentCommand(message.OrderId, message.CustomerId, message.TotalAmount, message.Currency));

        //   FOR Inbox State Testing: Uncomment the following lines to simulate a crash after processing the message
        // if (message.OrderId == Guid.Parse("00000000-0000-0000-0000-000000000005"))
        // Console.WriteLine("CRASH TEST: stopping process now");
        // Environment.Exit(1);

    }
}