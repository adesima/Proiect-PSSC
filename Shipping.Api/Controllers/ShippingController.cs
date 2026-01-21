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

        // Cautare după AWB (string)
        [HttpGet("{awb}")]
        public async Task<IActionResult> GetByAwb(string awb)
        {
            var awbCode = new AwbCode(awb);
            
            var shipment = await _repository.GetByAwbAsync(awbCode);
            
            if (shipment is null)
                return NotFound();

            // Returnez detaliile livrarii
            return Ok(new
            {
                Awb = shipment.Awb.Value,
                shipment.OrderId,
                shipment.CustomerId,
                Address = shipment.ShippingAddress.ToString(), 
                Cost = shipment.ShippingCost,
                shipment.ManifestedAt
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateShipment([FromBody] ShipOrderInput input)
        {
            // Mapez input-ul de la API la Value Objects din Domain
            var shippingAddress = new ShippingAddress(input.County, input.City, input.Street, input.PostalCode);

            var line = new ShipmentLine(
                productCode: input.ProductCode,
                quantity: input.Quantity
            );

            // Construiesc comanda
            var command = new ProcessShipmentCommand
            {
                OrderId = input.OrderId,
                CustomerId = input.CustomerId,
                ShippingAddress = shippingAddress,
                Lines = new[] { line }
            };

            // Execut Workflow-ul
            IShipmentManifestedEvent exportedEvent = _workflow.Execute(command);
            
            // Salvăm rezultatul
            await _repository.SaveAsync(exportedEvent);

            // Returnăm AWB-ul și costul calculat
            return Ok(new
            {
                Awb = exportedEvent.Awb,
                exportedEvent.OrderId,
                exportedEvent.CustomerId,
                ShippingCost = exportedEvent.ShippingCost, 
                exportedEvent.ManifestedAt
            });
        }
    }
}