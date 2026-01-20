using Domain.Invoicing.Models;

public record OrderPlacedMessage(
    Guid OrderId,
    Guid CustomerId,
    BillingAddress BillingAddress,
    IReadOnlyCollection<OrderLine> Lines,
    Money Amount,            // <-- new
    DateTime PlacedDate      // optional, if you need it
);
