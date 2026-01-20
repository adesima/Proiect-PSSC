using System;
using System.Collections.Generic;

namespace Domain.Sales.Models.Events // <--- Aici era probabil Sales.Api...
{
    public class OrderPlacedEvent
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public AddressEvent BillingAddress { get; set; }
        public List<OrderLineEvent> Lines { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PlacedDate { get; set; }
    }
}