/*using Domain.Invoicing.Models;
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
*/

using Azure.Messaging.ServiceBus;
using Billing.Api.Messaging;      
using Domain.Invoicing.Models;   // BillingAddress, OrderLine, Money

const string connectionString = "<SB_CONNECTION_STRING>";

const string topicName = "orders-confirmed";

var client = new ServiceBusClient(connectionString);
var sender = client.CreateSender(topicName);

var address = BillingAddress.Create("Strada Test 2", "Jimbolia", "300000");
var line = new OrderLine("TEST-PROD", 2, Money.Create(75m, "RON"));

var order = new OrderPlacedMessage(
    OrderId: Guid.NewGuid(),
    CustomerId: Guid.NewGuid(),
    BillingAddress: address,
    Lines: new List<OrderLine> { line });

var body = BinaryData.FromObjectAsJson(order);   
var message = new ServiceBusMessage(body);

await sender.SendMessageAsync(message);

Console.WriteLine("OrderPlacedMessage trimis.");

