using System;
using System.Collections.Generic;
using Domain.Shipping.Models;

namespace Domain.Shipping.Models
{
    public static class Shipment
    {
        // 1. Interfața Marker (Rădăcina)
        public interface IShipment { }

        // 2. Starea Inițială (Unvalidated)
        // Aici ajung datele brute din JSON-ul tău.
        public record UnvalidatedShipment : IShipment
        {
            public UnvalidatedShipment(
                Guid orderId,
                Guid customerId,
                ShippingAddress address,
                IReadOnlyCollection<ShipmentLine> lines)
            {
                OrderId = orderId;
                CustomerId = customerId;
                Address = address;
                Lines = lines;
            }

            public Guid OrderId { get; }
            public Guid CustomerId { get; }
            public ShippingAddress Address { get; }
            public IReadOnlyCollection<ShipmentLine> Lines { get; }
        }

        // 3. Starea de Eroare (Invalid)
        // Dacă adresa e goală sau nu sunt produse.
        public record InvalidShipment : IShipment
        {
            internal InvalidShipment(UnvalidatedShipment unvalidatedShipment, string reason)
            {
                UnvalidatedShipment = unvalidatedShipment;
                Reason = reason;
            }

            public UnvalidatedShipment UnvalidatedShipment { get; }
            public string Reason { get; }
        }

        // 4. Starea Validată (Validated)
        // Datele sunt corecte logic, putem calcula prețul.
        public record ValidatedShipment : IShipment
        {
            internal ValidatedShipment(
                Guid orderId,
                Guid customerId,
                ShippingAddress address,
                IReadOnlyCollection<ShipmentLine> lines)
            {
                OrderId = orderId;
                CustomerId = customerId;
                Address = address;
                Lines = lines;
            }

            public Guid OrderId { get; }
            public Guid CustomerId { get; }
            public ShippingAddress Address { get; }
            public IReadOnlyCollection<ShipmentLine> Lines { get; }
        }

        // 5. Starea Calculată (Calculated)
        // Avem costul transportului.
        public record CalculatedShipment : IShipment
        {
            internal CalculatedShipment(
                Guid orderId,
                Guid customerId,
                ShippingAddress address,
                IReadOnlyCollection<ShipmentLine> lines,
                decimal shippingCost)
            {
                OrderId = orderId;
                CustomerId = customerId;
                Address = address;
                Lines = lines;
                ShippingCost = shippingCost;
            }

            public Guid OrderId { get; }
            public Guid CustomerId { get; }
            public ShippingAddress Address { get; }
            public IReadOnlyCollection<ShipmentLine> Lines { get; }
            public decimal ShippingCost { get; }
        }

        // 6. Starea Finală (Manifested)
        // Corespunde cu 'PublishedExam' din exemplu. Avem AWB.
        public record ManifestedShipment : IShipment
        {
            internal ManifestedShipment(
                Guid orderId,
                Guid customerId,
                ShippingAddress address,
                IReadOnlyCollection<ShipmentLine> lines,
                decimal shippingCost,
                AwbCode awb)
            {
                OrderId = orderId;
                CustomerId = customerId;
                Address = address;
                Lines = lines;
                ShippingCost = shippingCost;
                Awb = awb;
                ManifestedAt = DateTime.Now;
            }

            public Guid OrderId { get; }
            public Guid CustomerId { get; }
            public ShippingAddress Address { get; }
            public IReadOnlyCollection<ShipmentLine> Lines { get; }
            public decimal ShippingCost { get; }
            public AwbCode Awb { get; }
            public DateTime ManifestedAt { get; }
        }
    }
}