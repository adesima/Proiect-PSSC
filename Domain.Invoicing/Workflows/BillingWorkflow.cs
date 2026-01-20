using Domain.Invoicing.Models;
using Domain.Invoicing.Operations;

namespace Domain.Invoicing.Workflows
{
    public class BillingWorkflow
    {
        public IInvoicePaidEvent Execute(
            GenerateInvoiceDraftCommand command,
            PaymentConfirmedEvent payment)
        {
            // draft
            UnvalidatedInvoice unvalidated =
                new GenerateInvoiceDraftOperation()
                    .Transform(command, null);

            // validated
            ValidatedInvoice validated =
                new ValidateInvoiceOperation()
                    .Transform(unvalidated, null);

            // calculated
            CalculatedInvoice calculated =
                new CalculateInvoiceTotalsOperation()
                    .Transform(validated, null);

            // paid
            PaidInvoice paid =
                new MarkInvoiceAsPaidOperation()
                    .Transform(calculated, payment);

            return paid.ToEvent();
        }
    }
}
