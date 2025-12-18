using Domain.Invoicing.Models;
using Domain.Invoicing.Operations;
using Domain.Invoicing.Workflows;

var command = new GenerateInvoiceDraftCommand
{
    OrderId = Guid.NewGuid(),
    CustomerId = Guid.NewGuid(),
    BillingAddress = BillingAddress.Create("Str. Test 1", "Cluj", "400000"),
    Lines = new[]
    {
        new OrderLine("SKU-123", 2, Money.Create(100m, "RON"))
    }
};

var payment = new PaymentConfirmedEvent(
    command.OrderId,
    Money.Create(238m, "RON"),  // 2 * 100 + 19% TVA
    DateTime.UtcNow);

var workflow = new BillingWorkflow();
var paidInvoice = workflow.Execute(command, payment);

Console.WriteLine($"Total factura: {paidInvoice.Total}");
Console.ReadLine();
