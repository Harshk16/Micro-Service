namespace Platform.Contracts.Products;

public sealed record ProductCreatedV1(
    Guid ProductId,
    string Name);