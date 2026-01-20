using System;
using System.Collections.Generic;
using Domain.Sales.Models.ValueObjects; 

namespace Domain.Sales.Models.Orders
{
    public class PlacedOrder : IOrder
    {
        public Guid OrderId { get; }
        public Guid ClientId { get; }
        public Address ShippingAddress { get; }
        public List<CalculatedOrderLine> Lines { get; }
        public decimal TotalAmount { get; }
        public DateTime PlacedDate { get; }

        public PlacedOrder(
            Guid orderId, 
            Guid clientId, 
            Address shippingAddress, 
            List<CalculatedOrderLine> lines, 
            decimal totalAmount, 
            DateTime placedDate)
        {
            OrderId = orderId;
            ClientId = clientId;
            ShippingAddress = shippingAddress;
            Lines = lines;
            TotalAmount = totalAmount;
            PlacedDate = placedDate;
        }
    }
}