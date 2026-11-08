namespace MicroService.Contracts.Events;

public record PaymentCompletedEvent(Guid OrderId, Guid PaymentId);