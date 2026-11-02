namespace Platform.Contracts.Inventory;

public sealed record InventoryUpdatedV1(
    Guid ProductId,
    int AvailableQuantity,
    DateTimeOffset UpdatedAt);
