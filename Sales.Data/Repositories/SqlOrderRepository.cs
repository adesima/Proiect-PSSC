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
            // 1. Mapăm din Domain Model (PlacedOrder) -> Entity Model (Tabela Orders)
            var orderEntity = new Models.OrderDto
            {
                OrderId = order.OrderId,
                ClientId = order.ClientId, // Acum avem coloana asta în baza de date!
                TotalAmount = order.TotalAmount,
                Currency = "RON", // Sau order.Currency dacă ai această proprietate
                PlacedDate = order.PlacedDate,

                // Mapăm Adresa (Value Object -> Coloane simple)
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

            // 3. Salvăm totul în baza de date (EF Core se ocupă de tranzacție)
            _dbContext.Orders.Add(orderEntity);
            await _dbContext.SaveChangesAsync();
        }
    }
}