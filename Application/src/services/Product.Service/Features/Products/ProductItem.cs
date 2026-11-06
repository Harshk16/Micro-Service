using System;

namespace Product.Service.Features.ProductItems;

public class ProductItem
{
    // Private setters protect the entity from being modified in an invalid state outside this class
    public Guid Id { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal PriceAmount { get; private set; }
    public string PriceCurrency { get; private set; } = "USD";
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    // Required by EF Core via reflection
    private ProductItem() { }

    // Factory method ensures an entity can only be instantiated with valid, required parameters
    public static ProductItem Create(Guid id, string sku, string name, decimal priceAmount, string description = "", string currency = "USD")
    {
        if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("SKU cannot be empty.", nameof(sku));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("ProductItem name cannot be empty.", nameof(name));
        if (priceAmount < 0) throw new ArgumentException("Price cannot be negative.", nameof(priceAmount));

        return new ProductItem
        {
            Id = id,
            Sku = sku.ToUpperInvariant().Trim(),
            Name = name.Trim(),
            PriceAmount = priceAmount,
            PriceCurrency = currency.ToUpperInvariant().Trim(),
            Description = description,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void Activate()
    {
        if (IsActive)
            throw new InvalidOperationException($"Product {Id} is already active.");

        IsActive = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException($"Product {Id} is already inactive.");

        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    // SKU and currency never change after creation
    public void UpdateDetails(string name, string? description, decimal priceAmount)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("ProductItem name cannot be empty.", nameof(name));
        if (priceAmount < 0) throw new ArgumentException("Price cannot be negative.", nameof(priceAmount));

        Name = name.Trim();
        Description = description;
        PriceAmount = priceAmount;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}