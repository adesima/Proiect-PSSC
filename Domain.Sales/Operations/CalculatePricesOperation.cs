using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Sales.Models.Orders;
using Domain.Sales.Models.ValueObjects;
using Domain.Sales.Repositories;

namespace Domain.Sales.Operations
{
    public class CalculatePricesOperation
    {
        private readonly IProductRepository _productRepository;

        public CalculatePricesOperation(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IOrder> Transform(IOrder order)
        {
            if (order is InvalidOrder) return order;

            if (order is not ValidatedOrder validatedOrder)
                return new InvalidOrder($"Stare invalida! Se astepta ValidatedOrder.");

            var calculatedLines = new List<CalculatedOrderLine>();
            decimal totalAmount = 0;

            foreach (var line in validatedOrder.Lines)
            {
                var price = await _productRepository.GetPriceAsync(line.Product);
                if (price == null) 
                    return new InvalidOrder($"Pretul nu a fost gasit pentru {line.Product}.");

                var lineTotal = price.Value * line.Quantity.Value;
                totalAmount += lineTotal;

                calculatedLines.Add(new CalculatedOrderLine(line.Product, line.Quantity, price.Value));
            }

            return new CalculatedOrder(
                calculatedLines, 
                new Money(totalAmount, "RON"), 
                validatedOrder.ShippingAddress);
        }
    }
}