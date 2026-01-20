// // using System;
// // using Domain.Shipping.Models;
// // using Domain.Shipping.Operations;
// // // using Microsoft.Extensions.Logging; // Optional, pentru logging
// //
// // namespace Domain.Shipping.Workflows
// // {
// //     public class ShippingWorkflow
// //     {
// //         // Workflow-ul orchestrează operațiile.
// //         // În mod normal, aceste operații ar putea fi injectate, dar aici le instanțiem direct
// //         // pentru simplitate, conform exemplelor de la laborator.
// //         
// //         public ManifestedShipment Execute(UnvalidatedShipment command)
// //         {
// //             // Pasul 1: Validare
// //             var validateOp = new ValidateShipmentOperation();
// //             ValidatedShipment validatedShipment = validateOp.Transform(command, null);
// //
// //             // Pasul 2: Calcul Cost
// //             var calculateOp = new CalculateShippingCostOperation();
// //             CalculatedShipment calculatedShipment = calculateOp.Transform(validatedShipment, null);
// //
// //             // Pasul 3: Generare AWB (Manifestare)
// //             var generateAwbOp = new GenerateAwbOperation();
// //             ManifestedShipment manifestedShipment = generateAwbOp.Transform(calculatedShipment, null);
// //
// //             return manifestedShipment;
// //         }
// //         
// //         // event
// //     }
// // }
//
//
//
// using System;
// using Domain.Shipping.Models;
// using Domain.Shipping.Operations;
// using static Domain.Shipping.Models.Shipment;
//
// namespace Domain.Shipping.Workflows
// {
//     public class ShippingWorkflow
//     {
//         public IShipmentEvent Execute(UnvalidatedShipment unvalidatedShipment)
//         {
//             // Pasul 1: Validare
//             // Input: Unvalidated -> Output: Validated SAU Invalid
//             IShipment shipment = new ValidateShipmentOperation().Transform(unvalidatedShipment);
//
//             // Pasul 2: Calcul Cost
//             // Executăm DOAR dacă pasul anterior a returnat un ValidatedShipment
//             if (shipment is ValidatedShipment validated)
//             {
//                 shipment = new CalculateShippingCostOperation().Transform(validated);
//             }
//
//             // Pasul 3: Generare AWB (Manifestare)
//             // Executăm DOAR dacă pasul anterior a returnat un CalculatedShipment
//             if (shipment is CalculatedShipment calculated)
//             {
//                 shipment = new GenerateAwbOperation().Transform(calculated);
//             }
//
//             // Pasul 4: Finalizare
//             // Transformăm starea finală (care poate fi Manifested sau Invalid) în Eveniment
//             return shipment.ToEvent();
//         }
//     }
// }



using Domain.Shipping.Models;
using Domain.Shipping.Operations;

namespace Domain.Shipping.Workflows
{
    public class ShippingWorkflow
    {
        // Metoda Execute primește doar comanda de start.
        // Nu mai avem nevoie de un 'PaymentConfirmedEvent' aici, 
        // pentru că AWB-ul îl generăm noi intern, nu așteptăm nimic din afară.
        public IShipmentManifestedEvent Execute(ProcessShipmentCommand command)
        {
            // 1. Procesare inițială (Command -> Unvalidated)
            UnvalidatedShipment unvalidated =
                new ProcessShipmentOperation()
                    .Transform(command, null);

            // 2. Validare (Unvalidated -> Validated)
            ValidatedShipment validated =
                new ValidateShipmentOperation()
                    .Transform(unvalidated, null);

            // 3. Calcul Cost (Validated -> Calculated)
            // Putem trece 'null' sau 'new object()' ca state, fiindcă nu folosim config extern momentan
            CalculatedShipment calculated =
                new CalculateShippingCostOperation()
                    .Transform(validated, null);

            // 4. Manifestare / Generare AWB (Calculated -> Manifested)
            // Aici transformarea se face automat, generând AWB-ul
            ManifestedShipment manifested =
                new ManifestShipmentOperation()
                    .Transform(calculated, null);

            return manifested.ToEvent();
        }
    }
}