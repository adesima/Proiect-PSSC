using Microsoft.AspNetCore.Mvc;
using Domain.Shipping.Models;
using Domain.Shipping.Workflows;
using Domain.Shipping.Repositories;
using Shipping.Data.Repositories;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Shipping.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShippingController : ControllerBase
    {
        // Endpoint-ul HTTP POST
        // Profesorul va trimite un JSON aici
        [HttpPost("process-shipment")]
        public IActionResult ProcessShipment([FromBody] ShipmentRequest request)
        {
            try
            {
                // 1. Transformăm datele de la API (JSON) în date de Domain
                var address = ShippingAddress.Create(
                    request.Street, 
                    request.City, 
                    request.PostalCode, 
                    request.County
                );

                var unvalidatedShipment = new UnvalidatedShipment
                {
                    OrderId = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    DeliveryAddress = address,
                    Lines = request.Lines.Select(l => new ShipmentLine(l.Product, l.Quantity)).ToList()
                };

                // 2. Apelăm Workflow-ul din Domain
                var workflow = new ShippingWorkflow();
                var result = workflow.Execute(unvalidatedShipment);

                // 3. Salvăm în "Baza de date"
                var repository = new ShipmentRepository();
                repository.SaveShipment(result);

                // 4. Returnăm răspunsul (200 OK) cu AWB-ul generat
                return Ok(new 
                { 
                    Message = "Comanda a fost procesata cu succes!",
                    Awb = result.Awb.Value,
                    Cost = result.ShippingCost,
                    GeneratedDate = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                // Dacă ceva e greșit (validare picată), returnăm 400 Bad Request
                return BadRequest(new { Error = ex.Message });
            }
        }
    }

    // Clasele ajutătoare pentru a citi JSON-ul (DTOs)
    public record ShipmentRequest(
        string Street, 
        string City, 
        string PostalCode, 
        string County, 
        List<ShipmentRequestLine> Lines
    );

    public record ShipmentRequestLine(string Product, int Quantity);
}