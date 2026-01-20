using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Sales.Models.Orders;
using Domain.Sales.Models.ValueObjects;

namespace Domain.Sales.Operations
{
    public class ValidateOrderOperation
    {
        private readonly Func<ProductCode, Task<bool>> _checkProductExists;
        private readonly Func<ProductCode, Quantity, Task<bool>> _checkStock;

        public ValidateOrderOperation(
            Func<ProductCode, Task<bool>> checkProductExists, 
            Func<ProductCode, Quantity, Task<bool>> checkStock)
        {
            _checkProductExists = checkProductExists;
            _checkStock = checkStock;
        }

        public async Task<IOrder> Transform(IOrder order)
        {
            // 1. Daca deja e eroare, o pasam mai departe (Short-circuit)
            if (order is InvalidOrder) return order;

            // 2. Verificam daca am primit starea corecta
            if (order is not UnvalidatedOrder unvalidatedOrder)
            {
                return new InvalidOrder($"Stare invalida! ValidateOrderOperation asteapta UnvalidatedOrder, nu {order.GetType().Name}.");
            }

            // 3. Logica de validare
            var validLines = new List<ValidatedOrderLine>();
            
            try 
            {
                // Recreăm adresa (validare format)
                var address = new ShippingAddress(
                    unvalidatedOrder.Body.ShippingAddress.City, 
                    unvalidatedOrder.Body.ShippingAddress.Street, 
                    unvalidatedOrder.Body.ShippingAddress.PostalCode);

                foreach (var line in unvalidatedOrder.Lines)
                {
                    if (!ProductCode.TryParse(line.ProductCode, out var productCode))
                        return new InvalidOrder($"Cod produs invalid: {line.ProductCode}");

                    var quantity = new Quantity(line.Quantity); // Valideaza cantitate > 0

                    // Verificari DB (prin functiile injectate)
                    if (!await _checkProductExists(productCode))
                        return new InvalidOrder($"Produsul {productCode} nu exista.");

                    if (!await _checkStock(productCode, quantity))
                        return new InvalidOrder($"Stoc insuficient pentru {productCode}.");

                    validLines.Add(new ValidatedOrderLine(productCode, quantity));
                }

                // 4. Returnam noua stare
                return new ValidatedOrder(validLines, address);
            }
            catch(Exception ex)
            {
                return new InvalidOrder($"Eroare validare: {ex.Message}");
            }
        }
    }
}