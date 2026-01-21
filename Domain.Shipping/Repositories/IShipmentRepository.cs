using System.Threading.Tasks;
using Domain.Shipping.Models;

namespace Domain.Shipping.Repositories
{
    public interface IShipmentRepository
    {
        // Returnează o livrare manifestata dupa codul AWB
        Task<ManifestedShipment?> GetByAwbAsync(AwbCode awb);

        // Salveaza starea finala în baza
        Task SaveAsync(IShipmentManifestedEvent shipmentEvent);
    }
}