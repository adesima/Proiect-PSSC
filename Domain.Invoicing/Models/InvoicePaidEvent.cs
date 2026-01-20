namespace Domain.Invoicing.Models;

public record InvoicePaidEvent
{
    public Guid InvoiceId { get; init; }
    public Guid OrderId { get; init; }
    public required Money Amount { get; init; }
    public DateTime PaidAt { get; init; }
}
