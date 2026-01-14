using System;
using System.Collections.Generic;
using Domain.Shipping.Models;       // Ca să vedem UnvalidatedShipment, ShippingAddress etc.
using Domain.Shipping.Workflows;    // Ca să vedem ShippingWorkflow

namespace Shipping.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== INITIALIZING SHIPPING WORKFLOW ===");

            // 1. Creăm datele de intrare (Simulăm o comandă venită de la Billing)
            var address = ShippingAddress.Create(
                street: "Bulevardul Timisoara 26",
                city: "Bucuresti",
                postalCode: "061331",
                county: "Sector 6"
            );

            var unvalidatedShipment = new UnvalidatedShipment
            {
                OrderId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                DeliveryAddress = address,
                Lines = new List<ShipmentLine>
                {
                    new ShipmentLine("IPHONE-15", 1),    // 1 telefon
                    new ShipmentLine("CASE-SILICONE", 2) // 2 huse
                }
            };

            // 2. Instanțiem Workflow-ul
            var workflow = new ShippingWorkflow();

            // 3. Executăm Workflow-ul
            try
            {
                Console.WriteLine("Processing shipment...");
                
                // Aici se întâmplă magia: Validare -> Calcul -> Generare AWB
                var result = workflow.Execute(unvalidatedShipment);

                Console.WriteLine("SHIPMENT PROCESSED SUCCESSFULLY!");
                Console.WriteLine("-----------------------------------");
                Console.WriteLine($"AWB Code:      {result.Awb}");
                Console.WriteLine($"Total Cost:    {result.ShippingCost} RON");
                Console.WriteLine($"To Address:    {result.DeliveryAddress}");
                Console.WriteLine("-----------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine("SHIPMENT FAILED!");
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Așteptăm input ca să nu se închidă consola imediat
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}