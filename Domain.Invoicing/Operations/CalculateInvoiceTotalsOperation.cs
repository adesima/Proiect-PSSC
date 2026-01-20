using Domain.Invoicing.Models;

namespace Domain.Invoicing.Operations;

public class CalculateInvoiceTotalsOperation
    : DomainOperation<ValidatedInvoice, object, CalculatedInvoice>
{
    public override CalculatedInvoice Transform(ValidatedInvoice invoice, object? _)
    {
        var subtotalAmount = invoice.Lines.Sum(line => line.UnitPrice.Amount * line.Quantity);
        var subtotal = Money.Create(subtotalAmount, invoice.Lines.First().UnitPrice.Currency);

        // For simplicity, we use a fixed tax rate of 19%
        var taxRate = TaxRate.Create(0.19m);
        var tax = taxRate.Apply(subtotal);
        var total = Money.Create(subtotal.Amount + tax.Amount, subtotal.Currency);

        return new CalculatedInvoice
        {
            OrderId = invoice.OrderId,
            CustomerId = invoice.CustomerId,
            BillingAddress = invoice.BillingAddress,
            Lines = invoice.Lines,
            Subtotal = subtotal,
            Tax = tax,
            Total = total
        };
    }
}
