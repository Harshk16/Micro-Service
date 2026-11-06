using Payment.Service.Common.Enums;

namespace Payment.Service.Features;

public class PaymentItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "USD";
    public ePaymentStatus Status { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    // Required for EF Core reflection
    private PaymentItem() { }

    public static PaymentItem Create(Guid id, Guid orderId, Guid customerId, decimal amount, string currency = "USD")
    {
        if (orderId == Guid.Empty) throw new ArgumentException("OrderId cannot be empty.", nameof(orderId));
        if (customerId == Guid.Empty) throw new ArgumentException("CustomerId cannot be empty.", nameof(customerId));
        if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency cannot be empty.", nameof(currency));

        return new PaymentItem
        {
            Id = id,
            OrderId = orderId,
            CustomerId = customerId,
            Amount = amount,
            Currency = currency.ToUpperInvariant().Trim(),
            Status = ePaymentStatus.Pending,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    // Only a pending payment can be approved
    public void Approve()
    {
        if (Status != ePaymentStatus.Pending)
            throw new InvalidOperationException($"Cannot approve a payment that is {Status}.");

        Status = ePaymentStatus.Approved;
        FailureReason = null;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    // Only a pending payment can be rejected, and a reason is required
    public void Reject(string reason)
    {
        if (Status != ePaymentStatus.Pending)
            throw new InvalidOperationException($"Cannot reject a payment that is {Status}.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Rejection reason is required.", nameof(reason));

        Status = ePaymentStatus.Rejected;
        FailureReason = reason.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }
}