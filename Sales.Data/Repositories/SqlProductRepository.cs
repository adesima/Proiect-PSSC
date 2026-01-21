using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Sales.Models.ValueObjects;
using Domain.Sales.Repositories;

namespace Sales.Data.Repositories
{
    public class SqlProductRepository : IProductRepository
    {
        private readonly SalesDbContext _dbContext;

        public SqlProductRepository(SalesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExistsAsync(ProductCode productCode)
        {
            return await _dbContext.Products
                .AnyAsync(p => p.ProductCode == productCode.Value);
        }

        public async Task<decimal?> GetPriceAsync(ProductCode productCode)
        {
            var product = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.ProductCode == productCode.Value);

            return product?.Price;
        }

        public async Task<bool> HasStockAsync(ProductCode productCode, Quantity quantity)
        {
            return await _dbContext.Products
                .AnyAsync(p => p.ProductCode == productCode.Value && p.Stoc >= quantity.Value);
        }

        // --- Aceasta este metoda care scade efectiv stocul ---
        public async Task<bool> TryReduceStockAsync(ProductCode productCode, Quantity quantity)
        {
            var sql = "UPDATE Products SET Stoc = Stoc - {0} WHERE ProductCode = {1} AND Stoc >= {0}";

            var rowsAffected = await _dbContext.Database.ExecuteSqlRawAsync(
                sql, 
                quantity.Value, 
                productCode.Value
            );

            return rowsAffected > 0;
        }
    }
}
