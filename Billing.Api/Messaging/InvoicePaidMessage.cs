using Domain.Invoicing.Models;

namespace Billing.Api.Messaging;

public record InvoicePaidMessage(
    Guid InvoiceId,
    Guid OrderId,
    Money Amount,
    DateTime PaidAt);
