using System.ComponentModel.DataAnnotations.Schema;
using Domain.Shipping.Models;

namespace Shipping.Data.Models
{
    public class ShipmentDto
    {
        // Primary Key în baza de date
        public Guid ShipmentId { get; set; } 

        // Codul AWB generat pentru expediere
        public string Awb { get; set; } 

        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }

        // Adresa de livrare 
        public string County { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        // Costul transportului
        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingCost { get; set; }
        public string Currency { get; set; } = "RON";

        // Data la care s-a generat AWB-ul
        public DateTime ManifestedAt { get; set; }
    }
}