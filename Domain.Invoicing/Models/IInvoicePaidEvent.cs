using Domain.Invoicing.Models;

public interface IInvoicePaidEvent
{
    Guid InvoiceId { get; }
    Guid OrderId { get; }
    Guid CustomerId { get; }
    public BillingAddress BillingAddress { get; init; }
    Money Subtotal { get; }
    Money Tax { get; }
    Money Total { get; }
    DateTime PaidAt { get; }
}

public record InvoicePaidEvent(
    Guid InvoiceId,
    Guid OrderId,
    Guid CustomerId,
    BillingAddress BillingAddress,
    Money Subtotal,
    Money Tax,
    Money Total,
    DateTime PaidAt
) : IInvoicePaidEvent;
