using System.Collections.Generic;
using Domain.Sales.Models.ValueObjects;

namespace Domain.Sales.Models.Orders
{
    public record ValidatedOrder(IEnumerable<ValidatedOrderLine> Lines, ShippingAddress ShippingAddress) : IOrder;

    public record ValidatedOrderLine(ProductCode Product, Quantity Quantity);
}