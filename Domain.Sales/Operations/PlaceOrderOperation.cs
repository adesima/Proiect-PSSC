using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Sales.Models.Events;
using Domain.Sales.Models.Orders;
using Domain.Sales.Repositories;
using Domain.Sales.Services;
using Domain.Sales.Models.ValueObjects;

namespace Domain.Sales.Operations
{
    public class PlaceOrderOperation
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IEventSender _eventSender;
        private readonly Guid _clientId;

        public PlaceOrderOperation(IOrderRepository orderRepository, IProductRepository productRepository, IEventSender eventSender, Guid clientId)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _eventSender = eventSender;
            _clientId = clientId;
        }

        public async Task<IOrder> Transform(IOrder order)
        {
            if (order is InvalidOrder) return order;

            if (order is not CalculatedOrder calculatedOrder)
                return new InvalidOrder($"Stare invalida! Se astepta CalculatedOrder.");

            // 1. Reducem stocul efectiv (DB Update)
            foreach (var line in calculatedOrder.Lines)
            {
                var success = await _productRepository.TryReduceStockAsync(line.Product, line.Quantity);
                if (!success) return new InvalidOrder($"Stocul s-a modificat intre timp pentru {line.Product}");
            }
            
            var finalAddress = new Address(
                calculatedOrder.ShippingAddress.County,
                calculatedOrder.ShippingAddress.City,
                calculatedOrder.ShippingAddress.Street,
                calculatedOrder.ShippingAddress.PostalCode
            );

            // 2. Creăm starea finală
            var placedOrder = new PlacedOrder(
                Guid.NewGuid(),
                _clientId,
                finalAddress,
                calculatedOrder.Lines.ToList(),
                calculatedOrder.TotalPrice.Amount,
                DateTime.UtcNow
            );

            // 3. Salvăm și trimitem evenimentul
            await _orderRepository.SaveOrderAsync(placedOrder);

            var eventToSend = new OrderPlacedEvent
            {
                OrderId = placedOrder.OrderId,
                CustomerId = placedOrder.ClientId,
                Amount = new MoneyEvent 
                { 
                    Amount = placedOrder.TotalAmount, 
                    Currency = "RON" 
                },
                PlacedDate = placedOrder.PlacedDate,
                Lines = placedOrder.Lines.Select(l => new OrderLineEvent { 
                    ProductCode = l.Product.Value, 
                    Quantity = l.Quantity.Value, 
                    UnitPrice = new MoneyEvent { Amount = l.Price, Currency = "RON" } 
                }).ToList(),
                BillingAddress = new AddressEvent { 
                    County = placedOrder.ShippingAddress.County,
                    City = placedOrder.ShippingAddress.City, 
                    Street = placedOrder.ShippingAddress.Street,
                    PostalCode = placedOrder.ShippingAddress.PostalCode 
                }
            };

            await _eventSender.SendAsync("orders-confirmed", eventToSend);

            return placedOrder;
        }
    }
}