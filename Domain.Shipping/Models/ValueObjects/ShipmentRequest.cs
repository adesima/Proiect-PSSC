using System;
using System.Collections.Generic;

namespace Shipping.Api.Models
{
    // Clasa principală care mapează JSON-ul de intrare
    public class ShipmentRequest
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public AddressDto ShippingAddress { get; set; }
        public List<ShipmentLineDto> Lines { get; set; }
    }

    public class AddressDto
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string PostalCode { get; set; }
    }

    public class ShipmentLineDto
    {
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
        // Ignorăm UnitPrice din JSON-ul mare, că nu ne trebuie
    }
}