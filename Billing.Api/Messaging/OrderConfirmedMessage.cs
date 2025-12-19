using Domain.Invoicing.Models;

public record OrderPlacedMessage(
    Guid OrderId,
    Guid CustomerId,
    BillingAddress BillingAddress,
    List<OrderLine> Lines);
