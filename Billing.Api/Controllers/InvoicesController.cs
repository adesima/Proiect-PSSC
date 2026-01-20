using Billing.Api.Models;
using Domain.Invoicing.Models;
using Domain.Invoicing.Operations;
using Domain.Invoicing.Repositories;
using Domain.Invoicing.Workflows; 
using Microsoft.AspNetCore.Mvc;

namespace Billing.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly BillingWorkflow _workflow;
        private readonly IInvoicesRepository _repository;

        public InvoicesController(BillingWorkflow workflow, IInvoicesRepository repository)
        {
            _workflow = workflow;
            _repository = repository;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var invoice = await _repository.GetByIdAsync(id);
            if (invoice is null)
                return NotFound();

            return Ok(new
            {
                invoice.InvoiceId,
                invoice.OrderId,
                invoice.CustomerId,
                Subtotal = invoice.Subtotal.Amount,
                Tax = invoice.Tax.Amount,
                Total = invoice.Total.Amount,
                Currency = invoice.Total.Currency,
                invoice.PaidAt
            });
        }

        [HttpPost]
        public async Task<IActionResult> PayInvoice([FromBody] PayInvoiceInput input)
        {
            var billingAddress = BillingAddress.Create(input.County, input.City, input.Street, input.PostalCode);

            var unitPrice = Money.Create(input.UnitPrice, input.Currency);

            var line = new OrderLine(
                productCode: input.ProductCode,
                quantity: input.Quantity,
                unitPrice: unitPrice
            );

            var command = new GenerateInvoiceDraftCommand
            {
                OrderId = input.OrderId,
                CustomerId = input.CustomerId,
                BillingAddress = billingAddress,
                Lines = new[] { line }
            };

            var subtotal = Money.Create(input.UnitPrice * input.Quantity, input.Currency);

            // TVA 19% 
            var taxRate = TaxRate.Create(0.19m);
            var tax = taxRate.Apply(subtotal);

            var totalWithVat = subtotal + tax;

            var payment = new PaymentConfirmedEvent(
                OrderId: input.OrderId,
                Amount: totalWithVat,
                PaidAt: DateTime.UtcNow
            );

            IInvoicePaidEvent paid = _workflow.Execute(command, payment);
            await _repository.SaveAsync(paid);

            return Ok(new
            {
                paid.InvoiceId,
                paid.OrderId,
                paid.CustomerId,
                Subtotal = paid.Subtotal.Amount,
                Tax = paid.Tax.Amount,
                Total = paid.Total.Amount,
                Currency = paid.Total.Currency,
                paid.PaidAt
            });
        }
    }
}
