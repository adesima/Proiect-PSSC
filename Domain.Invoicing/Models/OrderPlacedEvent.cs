namespace Domain.Invoicing.Models;

public record OrderPlacedEvent
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public required BillingAddress BillingAddress { get; init; }
    public IReadOnlyCollection<OrderLine> Lines { get; init; } = Array.Empty<OrderLine>();
}

public record OrderLine(string ProductCode, int Quantity, Money UnitPrice);
