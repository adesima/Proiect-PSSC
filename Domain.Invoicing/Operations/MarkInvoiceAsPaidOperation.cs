using Domain.Invoicing.Models;

namespace Domain.Invoicing.Operations;

public record PaymentConfirmedEvent(Guid OrderId, Money Amount, DateTime PaidAt);

public class MarkInvoiceAsPaidOperation
    : DomainOperation<CalculatedInvoice, PaymentConfirmedEvent, PaidInvoice>
{
    public override PaidInvoice Transform(CalculatedInvoice invoice, PaymentConfirmedEvent? payment)
    {
        if (payment is null)
            throw new ArgumentNullException(nameof(payment));

        if (payment.OrderId != invoice.OrderId)
            throw new ArgumentException("Payment does not match invoice order.");

        if (payment.Amount.Amount != invoice.Total.Amount || payment.Amount.Currency != invoice.Total.Currency)
            throw new ArgumentException("Payment amount does not match invoice total.");

        return new PaidInvoice
        {
            InvoiceId = Guid.NewGuid(),
            OrderId = invoice.OrderId,
            CustomerId = invoice.CustomerId,
            BillingAddress = invoice.BillingAddress,
            Lines = invoice.Lines,
            Subtotal = invoice.Subtotal,
            Tax = invoice.Tax,
            Total = invoice.Total,
            PaidAt = payment.PaidAt
        };
    }
}
