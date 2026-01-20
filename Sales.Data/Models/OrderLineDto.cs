using System;

namespace Sales.Data.Models
{
    public class OrderLineDto
    {
        public int OrderLineId { get; set; }
        public Guid OrderId { get; set; } // Cheia externă (Foreign Key)

        public string ProductCode { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }

        // Relația cu părintele (O linie aparține unei singure comenzi)
        public OrderDto Order { get; set; }
    }
}