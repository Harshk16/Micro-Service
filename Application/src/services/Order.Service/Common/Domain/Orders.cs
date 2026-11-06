using Order.Api.Common.Enums;

namespace Order.Api.Domain.Entities;

public class Orders
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public string Currency { get; private set; } = "USD";
    public eOrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string? CancellationReason { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    // Required for EF Core reflection
    private Orders() { }

    public static Orders Create(Guid id, Guid customerId, string currency = "USD")
    {
        if (customerId == Guid.Empty) throw new ArgumentException("CustomerId cannot be empty.", nameof(customerId));
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency cannot be empty.", nameof(currency));

        return new Orders
        {
            Id = id,
            CustomerId = customerId,
            Currency = currency.ToUpperInvariant().Trim(),
            Status = eOrderStatus.Pending,
            TotalAmount = 0,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    // Items can only be added while the order is Pending
    public void AddItem(Guid productId, string sku, int quantity, decimal unitPrice)
    {
        if (Status != eOrderStatus.Pending)
            throw new InvalidOperationException($"Cannot add items to an order that is {Status}.");

        if (_items.Any(x => x.ProductId == productId))
            throw new InvalidOperationException($"Product {productId} is already in this order.");

        _items.Add(OrderItem.Create(Guid.NewGuid(), Id, productId, sku, quantity, unitPrice));
        TotalAmount = _items.Sum(x => x.LineTotal);
    }

    // Payment approved (the saga calls this later)
    public void Confirm()
    {
        if (Status != eOrderStatus.Pending)
            throw new InvalidOperationException($"Cannot confirm an order that is {Status}.");
        if (_items.Count == 0)
            throw new InvalidOperationException("Cannot confirm an order with no items.");

        Status = eOrderStatus.Confirmed;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    // Payment rejected or stock unavailable (the saga calls this later)
    public void Cancel(string reason)
    {
        if (Status != eOrderStatus.Pending)
            throw new InvalidOperationException($"Cannot cancel an order that is {Status}.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Cancellation reason is required.", nameof(reason));

        Status = eOrderStatus.Cancelled;
        CancellationReason = reason.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }
}