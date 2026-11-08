namespace MicroService.Contracts.Events;

public record StockReservationFailedEvent(Guid OrderId, string Reason);