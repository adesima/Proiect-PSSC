using Domain.Shipping.Models;

namespace Domain.Shipping.Repositories
{
    public interface IShipmentRepository
    {
        // Metoda care va salva expedierea finalizată în baza de date
        void SaveShipment(ManifestedShipment shipment);
    }
}