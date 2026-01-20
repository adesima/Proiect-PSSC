namespace Domain.Invoicing.Models;

public record UnvalidatedInvoice
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public required BillingAddress BillingAddress { get; init; }
    public IReadOnlyCollection<OrderLine> Lines { get; init; } = Array.Empty<OrderLine>();
}
