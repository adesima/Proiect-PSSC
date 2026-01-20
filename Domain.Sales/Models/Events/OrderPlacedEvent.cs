using System;
using System.Collections.Generic;

namespace Domain.Sales.Models.Events
{
    public class OrderPlacedEvent
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public AddressEvent BillingAddress { get; set; } 
        public List<OrderLineEvent> Lines { get; set; }
        
        public MoneyEvent Amount { get; set; } 
        
        public DateTime PlacedDate { get; set; }
    }
}