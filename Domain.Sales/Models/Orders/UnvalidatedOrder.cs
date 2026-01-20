using System;
using System.Collections.Generic;

namespace Domain.Sales.Models.Orders
{
    public record UnvalidatedOrder(UnvalidatedOrderBody Body) : IOrder
    {
        public Guid ClientId => Body.ClientId;
        public List<UnvalidatedOrderLine> Lines => Body.Lines;
        public UnvalidatedAddress ShippingAddress => Body.ShippingAddress;
    }
    
    public record UnvalidatedOrderBody(
        Guid ClientId,
        List<UnvalidatedOrderLine> Lines,
        UnvalidatedAddress ShippingAddress
    );
    
    public record UnvalidatedOrderLine(string ProductCode, int Quantity);
    
    public record UnvalidatedAddress(string County,string City, string Street, string PostalCode);
}