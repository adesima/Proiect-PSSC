using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Domain.Sales.Models.Orders;
using Domain.Sales.Models.Events;
using Domain.Sales.Workflows;
using Sales.Api.Models;

namespace Sales.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly PlaceOrderWorkflow _placeOrderWorkflow;

        public OrdersController(ILogger<OrdersController> logger, PlaceOrderWorkflow placeOrderWorkflow)
        {
            _logger = logger;
            _placeOrderWorkflow = placeOrderWorkflow;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            var command = new UnvalidatedOrder(new UnvalidatedOrderBody(
                request.ClientId, 
                request.Lines.Select(l => new UnvalidatedOrderLine(l.ProductCode, l.Quantity)).ToList(),
                new UnvalidatedAddress(request.ShippingAddress.County, request.ShippingAddress.City, request.ShippingAddress.Street, request.ShippingAddress.PostalCode)
            ));

            IOrder result = await _placeOrderWorkflow.ExecuteAsync(command);

            return result switch
            {
                PlacedOrder placedOrder => Ok(placedOrder),
                InvalidOrder invalidOrder => BadRequest(invalidOrder.Reason),
                _ => StatusCode(500, "Unknown state")
            };
        }
    }
}