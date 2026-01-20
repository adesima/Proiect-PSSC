using System;
using System.Collections.Generic;
using Sales.Data.Models; 

namespace Sales.Api.Models
{
    public class PlaceOrderRequest
    {
        public Guid ClientId { get; set; } 
        public AddressDto ShippingAddress { get; set; }
        public List<OrderLineDto> Lines { get; set; }
    }

    public class AddressDto
    {
        public string County { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }

    public class OrderLineDto
    {
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
    }
}