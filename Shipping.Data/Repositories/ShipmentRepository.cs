using Domain.Shipping.Models;
using Domain.Shipping.Repositories;
using System.Collections.Generic;
using System;

namespace Shipping.Data.Repositories
{
    public class ShipmentRepository : IShipmentRepository
    {
        // Aceasta este "baza noastră de date" temporară
        private static readonly List<ManifestedShipment> _database = new();

        public void SaveShipment(ManifestedShipment shipment)
        {
            _database.Add(shipment);
            Console.WriteLine($"[DATA] Shipment {shipment.Awb} a fost salvat in baza de date.");
        }
    }
}