using Domain.Invoicing.Models;

public class GenerateInvoiceDraftCommand
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public BillingAddress BillingAddress { get; set; } = default!;
    public IReadOnlyCollection<OrderLine> Lines { get; set; } = Array.Empty<OrderLine>();

    public Money OrderAmount { get; set; } = default!;   // from Orders
    public DateTime PlacedDate { get; set; }             // from Orders
}
