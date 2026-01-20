using System;

namespace Billing.Api.Models
{
    public class PayInvoiceInput
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }

        public string County { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        public string ProductCode { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; } = 1;
        public string Currency { get; set; } = "RON";
    }
}
