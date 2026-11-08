namespace MicroService.Contracts.Events;

public record PaymentFailedEvent(Guid OrderId, string Reason);