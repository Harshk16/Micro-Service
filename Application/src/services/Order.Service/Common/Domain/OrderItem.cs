namespace Order.Api.Domain.Entities;   // keep your project's namespace

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    // calculated, not stored in the database
    public decimal LineTotal => Quantity * UnitPrice;

    // Required for EF Core reflection
    private OrderItem() { }

    public static OrderItem Create(Guid id, Guid orderId, Guid productId, string sku, int quantity, decimal unitPrice)
    {
        if (orderId == Guid.Empty) throw new ArgumentException("OrderId cannot be empty.", nameof(orderId));
        if (productId == Guid.Empty) throw new ArgumentException("ProductId cannot be empty.", nameof(productId));
        if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("SKU cannot be empty.", nameof(sku));
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        if (unitPrice < 0) throw new ArgumentException("Unit price cannot be negative.", nameof(unitPrice));

        return new OrderItem
        {
            Id = id,
            OrderId = orderId,
            ProductId = productId,
            Sku = sku.ToUpperInvariant().Trim(),
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }
}