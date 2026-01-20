using System.Threading.Tasks;
using Domain.Sales.Models.Orders;
using Domain.Sales.Operations;
using Domain.Sales.Repositories;
using Domain.Sales.Services;
using Microsoft.Extensions.Logging;

namespace Domain.Sales.Workflows
{
    public class PlaceOrderWorkflow
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IEventSender _eventSender;
        private readonly ILogger<PlaceOrderWorkflow> _logger;

        public PlaceOrderWorkflow(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IEventSender eventSender,
            ILogger<PlaceOrderWorkflow> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _eventSender = eventSender;
            _logger = logger;
        }

        public async Task<IOrder> ExecuteAsync(UnvalidatedOrder unvalidatedOrder)
        {
            // Initializam operatiile
            var validateOp = new ValidateOrderOperation(
                async (code) => await _productRepository.ExistsAsync(code),
                async (code, qty) => await _productRepository.HasStockAsync(code, qty)
            );

            var calculateOp = new CalculatePricesOperation(_productRepository);
            
            var placeOp = new PlaceOrderOperation(
                _orderRepository, 
                _productRepository, 
                _eventSender, 
                unvalidatedOrder.ClientId
            );
           
            IOrder order = unvalidatedOrder;
            
            order = await validateOp.Transform(order);
            order = await calculateOp.Transform(order);
            order = await placeOp.Transform(order);

            return order;
        }
    }
}