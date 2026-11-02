namespace Platform.Contracts.Payments;

public sealed record PaymentCompletedV1(
    Guid PaymentId,
    Guid OrderId,
    DateTimeOffset CompletedAt);
