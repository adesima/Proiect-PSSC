using System.Threading.Tasks;
using Domain.Sales.Models.Orders;
using Domain.Sales.Repositories;

namespace Sales.Data.Repositories
{
    public class SqlOrderRepository : IOrderRepository
    {
        private readonly SalesDbContext _dbContext;

        public SqlOrderRepository(SalesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveOrderAsync(PlacedOrder order)
        {
            var orderEntity = new Models.OrderDto
            {
                OrderId = order.OrderId,
                ClientId = order.ClientId, 
                TotalAmount = order.TotalAmount,
                Currency = "RON", 
                PlacedDate = order.PlacedDate,

                // Mapăm Adresa (Value Object -> Coloane simple)
                ShippingCounty = order.ShippingAddress.County,
                ShippingCity = order.ShippingAddress.City,
                ShippingStreet = order.ShippingAddress.Street,
                ShippingPostalCode = order.ShippingAddress.PostalCode
            };

            // 2. Mapăm Liniile (List<CalculatedOrderLine> -> Tabela OrderLines)
            foreach (var line in order.Lines)
            {
                orderEntity.OrderLines.Add(new Models.OrderLineDto
                {
                    // Folosim .Value pentru că în Domain sunt Value Objects, dar în DB sunt string/int
                    ProductCode = line.Product.Value, 
                    Quantity = line.Quantity.Value,
                    
                    UnitPrice = line.Price,
                    LineTotal = line.Price * line.Quantity.Value
                });
            }

            _dbContext.Orders.Add(orderEntity);
            await _dbContext.SaveChangesAsync();
        }
    }
}