// // using Domain.Shipping.Models;
// //
// // namespace Domain.Shipping.Repositories
// // {
// //     public interface IShipmentRepository
// //     {
// //         // Metoda care va salva expedierea finalizată în baza de date
// //         void SaveShipment(ManifestedShipment shipment);
// //     }
// // }
//
// using Domain.Shipping.Models;
// using System.Collections.Generic;
//
// namespace Domain.Shipping.Repositories
// {
//     public interface IShipmentRepository
//     {
//         // Vrem să salvăm doar livrările reușite (Manifested)
//         // sau putem salva interfața generică IShipment dacă vrem istoric.
//         // Pentru simplitate, salvăm doar ce e gata de livrare.
//         void SaveShipment(Shipment.ManifestedShipment shipment);
//
//         // Putem adăuga și o metodă de citire, utilă pentru verificări
//         // IEnumerable<ManifestedShipment> GetAll();
//     }
// }


using System.Threading.Tasks;
using Domain.Shipping.Models;

namespace Domain.Shipping.Repositories
{
    public interface IShipmentRepository
    {
        // Returnează o livrare manifestată după codul AWB
        // (Colegul tău avea Guid invoiceId, tu ai string awb)
        Task<ManifestedShipment?> GetByAwbAsync(AwbCode awb);

        // Salvează evenimentul/starea finală în bază
        // (La fel ca la colegul tău care salvează IInvoicePaidEvent)
        Task SaveAsync(IShipmentManifestedEvent shipmentEvent);
    }
}