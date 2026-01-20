// // using Domain.Shipping.Models;
// // using Domain.Shipping.Repositories;
// // using System.Collections.Generic;
// // using System;
// //
// // namespace Shipping.Data.Repositories
// // {
// //     public class ShipmentRepository : IShipmentRepository
// //     {
// //         // Aceasta este "baza noastră de date" temporară
// //         private static readonly List<ManifestedShipment> _database = new();
// //
// //         public void SaveShipment(ManifestedShipment shipment)
// //         {
// //             _database.Add(shipment);
// //             Console.WriteLine($"[DATA] Shipment {shipment.Awb} a fost salvat in baza de date.");
// //         }
// //     }
// // }
//
//
// using System.Threading.Tasks;
// using Shipping.Data.Models;
// using Domain.Shipping.Models;
// using Domain.Shipping.Repositories;
// using Microsoft.EntityFrameworkCore;
//
// namespace Shipping.Data.Repositories
// {
//     public class ShipmentRepository : IShipmentRepository
//     {
//         private readonly ShippingContext _dbContext;
//
//         public ShipmentRepository(ShippingContext dbContext)
//         {
//             _dbContext = dbContext;
//         }
//
//         // Căutăm livrarea după codul AWB (String)
//         public async Task<ManifestedShipment?> GetByAwbAsync(string awb)
//         {
//             // Căutăm în baza de date după coloana Awb
//             var entity = await _dbContext.Shipments
//                 .AsNoTracking()
//                 .FirstOrDefaultAsync(s => s.Awb == awb);
//
//             if (entity is null)
//                 return null;
//
//             // Reconstruim obiectul de domeniu din datele plate (DTO)
//             return new ManifestedShipment
//             {
//                 // Recreăm Value Object-ul AwbCode
//                 Awb = new AwbCode(entity.Awb),
//                 
//                 OrderId = entity.OrderId,
//                 CustomerId = entity.CustomerId,
//                 
//                 // Reconstruim adresa din coloanele separate
//                 ShippingAddress = new ShippingAddress(
//                     entity.County,
//                     entity.City,
//                     entity.Street,
//                     entity.PostalCode),
//                 
//                 // Inițializăm liniile goale (ca la colegul tău)
//                 Lines = Array.Empty<ShipmentLine>(), 
//                 
//                 ShippingCost = entity.ShippingCost,
//                 ManifestedAt = entity.ManifestedAt
//             };
//         }
//
//         public Task<ManifestedShipment?> GetByAwbAsync(AwbCode awb)
//         {
//             throw new NotImplementedException();
//         }
//
//         public async Task SaveAsync(IShipmentManifestedEvent shipmentEvent)
//         {
//             // Verificăm dacă există deja o intrare cu acest AWB
//             // (Deși e puțin probabil la insert, e bine pentru siguranță/idempotency)
//             var entity = await _dbContext.Shipments
//                 .FirstOrDefaultAsync(s => s.Awb == shipmentEvent.Awb);
//
//             if (entity is null)
//             {
//                 entity = new ShipmentDto
//                 {
//                     // Generăm un ID intern de bază de date, dar cheia importantă rămâne AWB
//                     ShipmentId = Guid.NewGuid(),
//                     Awb = shipmentEvent.Awb // Salvăm string-ul din AWB
//                 };
//                 _dbContext.Shipments.Add(entity);
//             }
//
//             // Actualizăm restul câmpurilor
//             entity.OrderId = shipmentEvent.OrderId;
//             entity.CustomerId = shipmentEvent.CustomerId;
//
//             // Desfacem adresa (Flattening)
//             entity.County = shipmentEvent.ShippingAddress.County;
//             entity.City = shipmentEvent.ShippingAddress.City;
//             entity.Street = shipmentEvent.ShippingAddress.Street;
//             entity.PostalCode = shipmentEvent.ShippingAddress.PostalCode;
//
//             entity.ShippingCost = shipmentEvent.ShippingCost;
//             entity.Currency = "RON"; // Hardcodat sau extras din Money dacă folosești Money
//
//             entity.ManifestedAt = shipmentEvent.ManifestedAt;
//
//             await _dbContext.SaveChangesAsync();
//         }
//     }
// }

using System.Threading.Tasks;
using Shipping.Data.Models;
using Domain.Shipping.Models;
using Domain.Shipping.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Shipping.Data.Repositories
{
    public class ShipmentRepository : IShipmentRepository
    {
        private readonly ShippingContext _dbContext;

        public ShipmentRepository(ShippingContext dbContext)
        {
            _dbContext = dbContext;
        }

        // --- FIX AICI: Parametrul este 'AwbCode', nu 'string' ---
        public async Task<ManifestedShipment?> GetByAwbAsync(AwbCode awb)
        {
            // Extragem valoarea string din obiectul AwbCode pentru a căuta în DB
            var awbString = awb.Value; 

            var entity = await _dbContext.Shipments
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Awb == awbString);

            if (entity is null)
                return null;

            // Conversie Decimal (DB) -> Money (Domain)
            var shippingCost = Money.Create(entity.ShippingCost, entity.Currency);

            return new ManifestedShipment
            {
                // Conversie String (DB) -> AwbCode (Domain)
                Awb = new AwbCode(entity.Awb),
                
                OrderId = entity.OrderId,
                CustomerId = entity.CustomerId,
                ShippingAddress = new ShippingAddress(
                    entity.County,
                    entity.City,
                    entity.Street,
                    entity.PostalCode),
                Lines = Array.Empty<ShipmentLine>(),
                
                ShippingCost = shippingCost,
                ManifestedAt = entity.ManifestedAt
            };
        }

        public async Task SaveAsync(IShipmentManifestedEvent shipmentEvent)
        {
            // Aici shipmentEvent.Awb este string (dacă ai urmat sfatul anterior)
            // sau AwbCode. Dacă e AwbCode, folosim .Value
            
            // Verificăm dacă Awb este obiect sau string în event. 
            // Presupunând că în event ai pus 'string Awb':
            var awbValue = shipmentEvent.Awb; 

            var entity = await _dbContext.Shipments
                .FirstOrDefaultAsync(s => s.Awb == awbValue);

            if (entity is null)
            {
                entity = new ShipmentDto
                {
                    ShipmentId = Guid.NewGuid(),
                    Awb = awbValue
                };
                _dbContext.Shipments.Add(entity);
            }

            entity.OrderId = shipmentEvent.OrderId;
            entity.CustomerId = shipmentEvent.CustomerId;

            entity.County = shipmentEvent.ShippingAddress.County;
            entity.City = shipmentEvent.ShippingAddress.City;
            entity.Street = shipmentEvent.ShippingAddress.Street;
            entity.PostalCode = shipmentEvent.ShippingAddress.PostalCode;

            // Conversie Money (Domain) -> Decimal (DB)
            // Verifică dacă în IShipmentManifestedEvent ai 'Money ShippingCost'
            entity.ShippingCost = shipmentEvent.ShippingCost.Amount;
            entity.Currency = shipmentEvent.ShippingCost.Currency;

            entity.ManifestedAt = shipmentEvent.ManifestedAt;

            await _dbContext.SaveChangesAsync();
        }
    }
}