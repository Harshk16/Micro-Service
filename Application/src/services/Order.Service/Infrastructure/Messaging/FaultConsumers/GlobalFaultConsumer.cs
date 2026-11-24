using MassTransit;

namespace Order.Service.Infrastructure.Messaging.FaultConsumers;

public class GlobalFaultConsumer : IConsumer<Fault>
{
    public Task Consume(ConsumeContext<Fault> context)
    {
        var reason = context.Message.Exceptions.FirstOrDefault()?.Message;
        Console.WriteLine($"[FAULT] Failed: Reason={reason}, MessageId={context.Message.FaultedMessageId}");
        return Task.CompletedTask;
    }
}