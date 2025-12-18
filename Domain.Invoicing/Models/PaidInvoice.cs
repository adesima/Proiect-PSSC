namespace Domain.Invoicing.Models;

public record PaidInvoice
{
    public Guid InvoiceId { get; init; }
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public required BillingAddress BillingAddress { get; init; }
    public IReadOnlyCollection<OrderLine> Lines { get; init; } = Array.Empty<OrderLine>();

    public required Money Subtotal { get; init; }
    public required Money Tax { get; init; }
    public required Money Total { get; init; }

    public DateTime PaidAt { get; init; }
}
