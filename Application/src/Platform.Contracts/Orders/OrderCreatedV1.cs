namespace Platform.Contracts.Orders;

public sealed record OrderCreatedV1(
    Guid OrderId,
    DateTimeOffset CreatedAt);
