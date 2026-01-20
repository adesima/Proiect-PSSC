using System.Threading.Tasks;
using Domain.Sales.Models.Orders;

namespace Domain.Sales.Repositories
{
    public interface IOrderRepository
    {
        // Salvează comanda plasată
        Task SaveOrderAsync(PlacedOrder order);
    }
}