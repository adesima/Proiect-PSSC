using Domain.Invoicing.Models;
using Microsoft.Azure.Amqp.Framing;

namespace Billing.Api.Messaging;

public record InvoicePaidMessage(
    Guid OrderId,
    Guid CustomerId,
    Guid InvoiceId,
    BillingAddress ShippingAddress,          
    IReadOnlyCollection<OrderLine> Lines,
    Money Amount,                            // order amount from Sales
    DateTime PlacedDate,                     // from Sales
    Money TotalAmount,                       // billing total (with VAT)
    DateTime PaidAt);                        // from Billing);
