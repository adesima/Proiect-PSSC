using System;
using Domain.Shipping.Models;
using Domain.Shipping.Operations;
// using Microsoft.Extensions.Logging; // Optional, pentru logging

namespace Domain.Shipping.Workflows
{
    public class ShippingWorkflow
    {
        // Workflow-ul orchestrează operațiile.
        // În mod normal, aceste operații ar putea fi injectate, dar aici le instanțiem direct
        // pentru simplitate, conform exemplelor de la laborator.
        
        public ManifestedShipment Execute(UnvalidatedShipment command)
        {
            // Pasul 1: Validare
            var validateOp = new ValidateShipmentOperation();
            ValidatedShipment validatedShipment = validateOp.Transform(command, null);

            // Pasul 2: Calcul Cost
            var calculateOp = new CalculateShippingCostOperation();
            CalculatedShipment calculatedShipment = calculateOp.Transform(validatedShipment, null);

            // Pasul 3: Generare AWB (Manifestare)
            var generateAwbOp = new GenerateAwbOperation();
            ManifestedShipment manifestedShipment = generateAwbOp.Transform(calculatedShipment, null);

            return manifestedShipment;
        }
    }
}