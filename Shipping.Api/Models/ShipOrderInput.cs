namespace Shipping.Api.Models
{
    public class ShipOrderInput
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        
        // Adresa
        public string County { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        // Produsul (Simplificat la o singură linie pentru exemplu)
        public string ProductCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}