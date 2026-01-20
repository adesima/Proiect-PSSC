using System.Linq;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Billing.Api.Messaging;
using Domain.Invoicing.Models;
using Domain.Invoicing.Operations;
using Domain.Invoicing.Repositories;
using Domain.Invoicing.Workflows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

public class OrderPlacedListener : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly ServiceBusSender _paymentsSender;
    private readonly BillingWorkflow _workflow;
    private readonly IInvoicesRepository _repository;

    public OrderPlacedListener(
        ServiceBusClient client,
        IConfiguration config,
        BillingWorkflow workflow,
        IInvoicesRepository repository)
    {
        _workflow = workflow;
        _repository = repository;

        var ordersTopic = config["ServiceBus:OrdersTopic"];
        var billingSub = config["ServiceBus:BillingSubscription"];
        _processor = client.CreateProcessor(ordersTopic, billingSub);

        var paymentsTopic = config["ServiceBus:PaymentsTopic"];
        _paymentsSender = client.CreateSender(paymentsTopic);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor.ProcessMessageAsync += OnMessageAsync;
        _processor.ProcessErrorAsync += args =>
        {
            Console.WriteLine($"SB ERROR: {args.Exception.GetType().Name} - {args.Exception.Message}");
            return Task.CompletedTask;
        };

        return _processor.StartProcessingAsync(stoppingToken);
    }

    private async Task OnMessageAsync(ProcessMessageEventArgs args)
    {
        var order = args.Message.Body.ToObjectFromJson<OrderPlacedMessage>();

        if (order is null)
        {
            await args.DeadLetterMessageAsync(args.Message, "DeserializationFailed", "OrderPlacedMessage was null");
            return;
        }

        // 1. command 
        var command = new GenerateInvoiceDraftCommand
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            BillingAddress = order.BillingAddress,
            Lines = order.Lines,
            OrderAmount = order.Amount,
            PlacedDate = order.PlacedDate
        };

        // 2. subtotal + TVA (19%)
        var firstLine = order.Lines.First();
        var currency = firstLine.UnitPrice.Currency;

        var subtotalValue = order.Lines
            .Sum(l => l.UnitPrice.Amount * l.Quantity);

        var subtotal = Money.Create(subtotalValue, currency);
        var taxRate = TaxRate.Create(0.19m);
        var tax = taxRate.Apply(subtotal);
        var totalWithVat = subtotal + tax;

        var payment = new PaymentConfirmedEvent(
            OrderId: order.OrderId,
            Amount: totalWithVat,
            PaidAt: DateTime.UtcNow);

        IInvoicePaidEvent paidEvent = _workflow.Execute(command, payment);
        await _repository.SaveAsync(paidEvent);

        var paidMessage = new InvoicePaidMessage(
            OrderId: paidEvent.OrderId,
            CustomerId: command.CustomerId,
            InvoiceId: paidEvent.InvoiceId,
            ShippingAddress: command.BillingAddress, 
            Lines: command.Lines,
            Amount: command.OrderAmount,             // from Sales
            PlacedDate: command.PlacedDate,          // from Sales
            TotalAmount: paidEvent.Total,            // from Billing
            PaidAt: paidEvent.PaidAt);

        var body = BinaryData.FromObjectAsJson(paidMessage);
        await _paymentsSender.SendMessageAsync(new ServiceBusMessage(body));

        await args.CompleteMessageAsync(args.Message);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync(cancellationToken);
        await _processor.DisposeAsync();
        await _paymentsSender.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}
