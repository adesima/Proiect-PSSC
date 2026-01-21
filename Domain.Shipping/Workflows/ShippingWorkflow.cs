using Domain.Shipping.Models;
using Domain.Shipping.Operations;

namespace Domain.Shipping.Workflows
{
    public class ShippingWorkflow
    {
        public IShipmentManifestedEvent Execute(ProcessShipmentCommand command)
        {
            // Procesare inițială (Command -> Unvalidated)
            UnvalidatedShipment unvalidated =
                new ProcessShipmentOperation()
                    .Transform(command, null);

            // Validare (Unvalidated -> Validated)
            ValidatedShipment validated =
                new ValidateShipmentOperation()
                    .Transform(unvalidated, null);

            // Calcul Cost (Validated -> Calculated)
            CalculatedShipment calculated =
                new CalculateShippingCostOperation()
                    .Transform(validated, null);

            // Manifestare / Generare AWB (Calculated -> Manifested)
            ManifestedShipment manifested =
                new ManifestShipmentOperation()
                    .Transform(calculated, null);

            return manifested.ToEvent();
        }
    }
}