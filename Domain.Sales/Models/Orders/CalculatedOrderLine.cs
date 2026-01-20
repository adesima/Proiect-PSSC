using Domain.Sales.Models.ValueObjects; 

namespace Domain.Sales.Models.Orders
{
    public class CalculatedOrderLine
    {
        public ProductCode Product { get; }
        public Quantity Quantity { get; }
        public decimal Price { get; }

        public CalculatedOrderLine(ProductCode product, Quantity quantity, decimal price)
        {
            Product = product;
            Quantity = quantity;
            Price = price;
        }
    }
}