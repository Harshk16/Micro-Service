using System;

namespace Inventory.Service.Features.InventoryItems;

public class InventoryItem
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public int QuantityAvailable { get; private set; }
    public int QuantityReserved { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    // Required for EF Core reflection
    private InventoryItem() { }

    public static InventoryItem Create(Guid id, Guid productId, string sku, int initialStock)
    {
        if (productId == Guid.Empty) throw new ArgumentException("ProductId cannot be empty.", nameof(productId));
        if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("SKU cannot be empty.", nameof(sku));
        if (initialStock < 0) throw new ArgumentException("Initial stock cannot be negative.", nameof(initialStock));

        return new InventoryItem
        {
            Id = id,
            ProductId = productId,
            Sku = sku.ToUpperInvariant().Trim(),
            QuantityAvailable = initialStock,
            QuantityReserved = 0,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    // Manual stock change: positive adds, negative removes
    public void AdjustStock(int quantity)
    {
        if (QuantityAvailable + quantity < 0)
            throw new InvalidOperationException($"Insufficient stock. Current: {QuantityAvailable}, Attempted reduction: {Math.Abs(quantity)}");

        QuantityAvailable += quantity;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    // Order started: hold stock
    public void Reserve(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.", nameof(quantity));
        if (QuantityAvailable < quantity)
            throw new InvalidOperationException($"Cannot reserve {quantity}. Available: {QuantityAvailable}");

        QuantityAvailable -= quantity;
        QuantityReserved += quantity;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    // Payment failed or order cancelled: give stock back
    public void Release(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.", nameof(quantity));
        if (QuantityReserved < quantity)
            throw new InvalidOperationException($"Cannot release {quantity}. Reserved: {QuantityReserved}");

        QuantityReserved -= quantity;
        QuantityAvailable += quantity;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    // Payment succeeded: the reserved stock is sold
    public void Commit(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.", nameof(quantity));
        if (QuantityReserved < quantity)
            throw new InvalidOperationException($"Cannot commit {quantity}. Reserved: {QuantityReserved}");

        QuantityReserved -= quantity;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}