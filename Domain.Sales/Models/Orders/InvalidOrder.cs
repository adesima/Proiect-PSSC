using System.Collections.Generic;

namespace Domain.Sales.Models.Orders
{
    public record InvalidOrder(string Reason) : IOrder;
}