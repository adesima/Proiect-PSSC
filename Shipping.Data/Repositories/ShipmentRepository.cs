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
            entity.ShippingCost = shipmentEvent.ShippingCost.Amount;
            entity.Currency = shipmentEvent.ShippingCost.Currency;

            entity.ManifestedAt = shipmentEvent.ManifestedAt;

            await _dbContext.SaveChangesAsync();
        }
    }
}