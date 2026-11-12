namespace Inventory.Service.Common.Domain;

public class OrderItemReservation
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public bool IsReserved { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private OrderItemReservation() { }

    public static OrderItemReservation Create(Guid orderId, Guid productId, int quantity) => new()
    {
        Id = Guid.NewGuid(),
        OrderId = orderId,
        ProductId = productId,
        Quantity = quantity,
        IsReserved = false,
        CreatedAtUtc = DateTime.UtcNow
    };

    public void MarkReserved()
    {
        IsReserved = true;
    }
}