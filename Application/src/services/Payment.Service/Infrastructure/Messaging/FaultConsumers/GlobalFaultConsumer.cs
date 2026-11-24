using MassTransit;
using MicroService.Contracts.Events;

namespace Payment.Service.Infrastructure.Messaging.FaultConsumers;

// public class GlobalFaultConsumer : IConsumer<Fault>
// {
//     public Task Consume(ConsumeContext<Fault> context)
//     {
//         var reason = context.Message.Exceptions.FirstOrDefault()?.Message;
//         Console.WriteLine($"[FAULT] Failed: Reason={reason}, MessageId={context.Message.FaultedMessageId}");
//         return Task.CompletedTask;
//     }
// }

// public class OrderCreatedFaultConsumer : IConsumer<Fault<OrderCreatedEvent>>
// {
//     public Task Consume(ConsumeContext<Fault<OrderCreatedEvent>> context)
//     {
//         var reason = context.Message.Exceptions.FirstOrDefault()?.Message;
//         Console.WriteLine($"[FAULT] OrderCreated failed: OrderId={context.Message.Message.OrderId}, Reason={reason}");
//         return Task.CompletedTask;
//     }
// }

 public class GlobalFaultConsumer : IConsumer<Fault>
    {
        public async Task Consume(ConsumeContext<Fault> context)
        {
            var exceptionInfo = context.Message.Exceptions.FirstOrDefault();
            string errorMessage = exceptionInfo?.Message ?? "Unknown error";
            string messageType = context.SupportedMessageTypes.FirstOrDefault() ?? "Unknown type";

            Console.WriteLine($"=================== GLOBAL FAULT DETECTED ===================");
            Console.WriteLine($"[DLQ ALERT] Message failed permanently across the service.");
            Console.WriteLine($"Failed Contract Type : {messageType}");
            Console.WriteLine($"Error Reason         : {errorMessage}");
            Console.WriteLine($"Message ID           : {context.Message.FaultedMessageId}");
            Console.WriteLine($"=============================================================");

            await Task.CompletedTask;
        }
    }