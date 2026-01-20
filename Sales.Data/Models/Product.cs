using System.ComponentModel.DataAnnotations;

namespace Sales.Data.Models
{
    public class Product
    {
        [Key] // Spunem că asta e Cheia Primară
        public string ProductCode { get; set; }
        
        public int Stoc { get; set; }
        public decimal Price { get; set; }
    }
}