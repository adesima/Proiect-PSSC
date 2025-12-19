namespace Billing.Data.Models
{
    public class InvoiceDto
    {
        public Guid InvoiceId { get; set; } // PK
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }

        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        public decimal SubtotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "RON";

        public DateTime PaidAt { get; set; }
        public bool IsPaid { get; set; }
    }
}
