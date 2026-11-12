namespace MicroService.Contracts.Events;

// public record OrderCreatedEvent(Guid OrderId, Guid CustomerId, decimal TotalAmount, string Currency);

public record OrderCreatedEvent(
    Guid OrderId,
    Guid CustomerId,
    decimal TotalAmount,
    string Currency,
    List<OrderItemContract> Items);