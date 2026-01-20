using System.Threading.Tasks;
using Domain.Sales.Models.ValueObjects;

namespace Domain.Sales.Repositories
{
    public interface IProductRepository
    {
        // Verifică existența produsului în catalog
        Task<bool> ExistsAsync(ProductCode productCode);
        
        // Verifică dacă stocul este suficient
        Task<bool> HasStockAsync(ProductCode productCode, Quantity quantity);

        // Obține prețul (returnează null dacă produsul nu e găsit)
        Task<decimal?> GetPriceAsync(ProductCode productCode);
        Task<bool> TryReduceStockAsync(ProductCode productCode, Quantity quantity);
    }
}