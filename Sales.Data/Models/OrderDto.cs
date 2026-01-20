using System;
using System.Collections.Generic;

namespace Sales.Data.Models
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public Guid ClientId { get; set; } 
        
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        
        public string ShippingCounty { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingStreet { get; set; }
        public string ShippingPostalCode { get; set; }
        
        public DateTime PlacedDate { get; set; }

        public ICollection<OrderLineDto> OrderLines { get; set; } = new List<OrderLineDto>();
    }
}