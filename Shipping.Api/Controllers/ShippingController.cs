// // using Microsoft.AspNetCore.Mvc;
// // using Domain.Shipping.Models;
// // using Domain.Shipping.Workflows;
// // using Domain.Shipping.Repositories;
// // using Shipping.Data.Repositories;
// // using System.Collections.Generic;
// // using System.Linq;
// // using System;
// //
// // namespace Shipping.Api.Controllers
// // {
// //     [ApiController]
// //     [Route("api/[controller]")]
// //     public class ShippingController : ControllerBase
// //     {
// //         // Endpoint-ul HTTP POST
// //         // Profesorul va trimite un JSON aici
// //         [HttpPost("process-shipment")]
// //         public IActionResult ProcessShipment([FromBody] ShipmentRequest request)
// //         {
// //             try
// //             {
// //                 // 1. Transformăm datele de la API (JSON) în date de Domain
// //                 var address = ShippingAddress.Create(
// //                     request.Street, 
// //                     request.City, 
// //                     request.PostalCode, 
// //                     request.County
// //                 );
// //
// //                 var unvalidatedShipment = new UnvalidatedShipment
// //                 {
// //                     OrderId = Guid.NewGuid(),
// //                     CustomerId = Guid.NewGuid(),
// //                     DeliveryAddress = address,
// //                     Lines = request.Lines.Select(l => new ShipmentLine(l.Product, l.Quantity)).ToList()
// //                 };
// //
// //                 // 2. Apelăm Workflow-ul din Domain
// //                 var workflow = new ShippingWorkflow();
// //                 var result = workflow.Execute(unvalidatedShipment);
// //
// //                 // 3. Salvăm în "Baza de date"
// //                 var repository = new ShipmentRepository();
// //                 repository.SaveShipment(result);
// //
// //                 // 4. Returnăm răspunsul (200 OK) cu AWB-ul generat
// //                 return Ok(new 
// //                 { 
// //                     Message = "Comanda a fost procesata cu succes!",
// //                     Awb = result.Awb.Value,
// //                     Cost = result.ShippingCost,
// //                     GeneratedDate = DateTime.Now
// //                 });
// //             }
// //             catch (Exception ex)
// //             {
// //                 // Dacă ceva e greșit (validare picată), returnăm 400 Bad Request
// //                 return BadRequest(new { Error = ex.Message });
// //             }
// //         }
// //     }
// //
// //     // Clasele ajutătoare pentru a citi JSON-ul (DTOs)
// //     public record ShipmentRequest(
// //         string Street, 
// //         string City, 
// //         string PostalCode, 
// //         string County, 
// //         List<ShipmentRequestLine> Lines
// //     );
// //
// //     public record ShipmentRequestLine(string Product, int Quantity);
// // }
//
//
// using Microsoft.AspNetCore.Mvc;
// using System;
// using System.Linq;
// using System.Threading.Tasks;
// using Domain.Shipping.Models;
// // using Domain.Shipping.Models.ValueObjects;
// using Domain.Shipping.Repositories;
// using Domain.Shipping.Workflows;
// using Shipping.Api.Models;
// using static Domain.Shipping.Models.Shipment;
//
// namespace Shipping.Api.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class ShippingController : ControllerBase
//     {
//         private readonly IShipmentRepository _repository;
//
//         // Injectăm Repository-ul ca să îl dăm mai departe Workflow-ului
//         public ShippingController(IShipmentRepository repository)
//         {
//             _repository = repository;
//         }
//
//         [HttpPost("process")]
//         public IActionResult ProcessShipment([FromBody] ShipmentRequest request)
//         {
//             try
//             {
//                 // 1. Mapare DTO (API) -> Domain Model
//                 var address = new ShippingAddress(
//                     request.ShippingAddress.County,
//                     request.ShippingAddress.City,
//                     request.ShippingAddress.Street,
//                     request.ShippingAddress.PostalCode
//                 );
//
//                 var lines = request.Lines
//                     .Select(l => new ShipmentLine(l.ProductCode, l.Quantity))
//                     .ToList();
//
//                 var unvalidatedShipment = new UnvalidatedShipment(
//                     request.OrderId,
//                     request.CustomerId,
//                     address,
//                     lines
//                 );
//
//                 // 2. Instanțiere Workflow cu Repository injectat
//                 var workflow = new ShippingWorkflow(_repository);
//
//                 // 3. Execuție
//                 var result = workflow.Execute(unvalidatedShipment);
//
//                 // 4. Returnare Răspuns
//                 if (result is ShipmentFailedEvent failedEvent)
//                 {
//                     return BadRequest(new { Error = failedEvent.Reason });
//                 }
//
//                 if (result is ShipmentManifestedEvent successEvent)
//                 {
//                     // Returnăm 200 OK și detaliile AWB-ului
//                     return Ok(new
//                     {
//                         Message = "Expediere procesată cu succes!",
//                         Awb = successEvent.Awb,
//                         Cost = successEvent.ShippingCost,
//                         OrderId = successEvent.OrderId,
//                         Date = successEvent.ManifestedAt
//                     });
//                 }
//
//                 return StatusCode(500, "Stare necunoscută.");
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, new { Error = ex.Message });
//             }
//         }
//     }
// }


using Shipping.Api.Models; // Aici va fi ShipOrderInput
using Domain.Shipping.Models;
using Domain.Shipping.Repositories;
using Domain.Shipping.Workflows; 
using Microsoft.AspNetCore.Mvc;

namespace Shipping.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShippingController : ControllerBase
    {
        private readonly ShippingWorkflow _workflow;
        private readonly IShipmentRepository _repository;

        public ShippingController(ShippingWorkflow workflow, IShipmentRepository repository)
        {
            _workflow = workflow;
            _repository = repository;
        }

        // Căutare după AWB (string), nu după Guid
        [HttpGet("{awb}")]
        public async Task<IActionResult> GetByAwb(string awb)
        {
            var awbCode = new AwbCode(awb);
            
            var shipment = await _repository.GetByAwbAsync(awbCode);
            
            if (shipment is null)
                return NotFound();

            // Returnăm detaliile livrării
            return Ok(new
            {
                Awb = shipment.Awb.Value,
                shipment.OrderId,
                shipment.CustomerId,
                Address = shipment.ShippingAddress.ToString(), // Sau descompus
                Cost = shipment.ShippingCost,
                shipment.ManifestedAt
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateShipment([FromBody] ShipOrderInput input)
        {
            // 1. Mapăm input-ul de la API la Value Objects din Domain
            // Presupunem că ShippingAddress are o metodă similară cu BillingAddress
            var shippingAddress = new ShippingAddress(input.County, input.City, input.Street, input.PostalCode);

            var line = new ShipmentLine(
                productCode: input.ProductCode,
                quantity: input.Quantity
            );

            // 2. Construim comanda
            var command = new ProcessShipmentCommand
            {
                OrderId = input.OrderId,
                CustomerId = input.CustomerId,
                ShippingAddress = shippingAddress,
                Lines = new[] { line }
            };

            // 3. Executăm Workflow-ul
            // La noi e mai simplu: nu calculăm taxe manual în controller, workflow-ul face totul.
            IShipmentManifestedEvent exportedEvent = _workflow.Execute(command);
            
            // 4. Salvăm rezultatul
            await _repository.SaveAsync(exportedEvent);

            // 5. Returnăm AWB-ul și costul calculat
            return Ok(new
            {
                Awb = exportedEvent.Awb,
                exportedEvent.OrderId,
                exportedEvent.CustomerId,
                ShippingCost = exportedEvent.ShippingCost, // Costul calculat de logica ta
                exportedEvent.ManifestedAt
            });
        }
    }
}