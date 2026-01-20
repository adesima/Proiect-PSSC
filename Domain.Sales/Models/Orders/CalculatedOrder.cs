using System.Collections.Generic;
using Domain.Sales.Models.ValueObjects;

namespace Domain.Sales.Models.Orders
{
    public record CalculatedOrder(
        IEnumerable<CalculatedOrderLine> Lines,
        Money TotalPrice,
        ShippingAddress ShippingAddress
    ) : IOrder;
}