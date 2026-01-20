using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Domain.Shipping.Models;
using Domain.Shipping.Repositories;
using Domain.Shipping.Workflows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shipping.Api.Messaging;

namespace Shipping.Api;

public class InvoicePaidListener : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly ServiceBusSender _shippingSender;
    private readonly IServiceProvider _serviceProvider;

    public InvoicePaidListener(
        ServiceBusClient client,
        IConfiguration config,
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        // 1. CONFIGURARE CONSUMATOR (Input)
        // Ascultăm topicul unde colegul tău trimite mesajele (ex: "sbt-billing-payments")
        var billingTopic = config["ServiceBus:BillingTopic"];
        var shippingSubscription = config["ServiceBus:ShippingSubscription"];
        
        _processor = client.CreateProcessor(billingTopic, shippingSubscription, new ServiceBusProcessorOptions
        {
            // Setări pentru siguranță (să nu proceseze prea multe deodată)
            MaxConcurrentCalls = 1,
            AutoCompleteMessages = false
        });

        // 2. CONFIGURARE PRODUCĂTOR (Output)
        // Trimitem mesajul final către un topic nou (ex: "sbt-shipping-manifests")
        var shippingTopic = config["ServiceBus:ShippingTopic"];
        _shippingSender = client.CreateSender(shippingTopic);
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
        // A. Deserializare Mesaj Intrare (InvoicePaidMessage)
        var bodyJson = args.Message.Body.ToString();
        var invoiceMsg = JsonSerializer.Deserialize<InvoicePaidMessage>(bodyJson);
        // SAU varianta Azure SDK: args.Message.Body.ToObjectFromJson<InvoicePaidMessage>();

        if (invoiceMsg is null)
        {
            await args.DeadLetterMessageAsync(args.Message, "DeserializationFailed", "Body was null");
            return;
        }

        // B. Creare Scope (Pentru servicii Scoped: DB, Workflow)
        using (var scope = _serviceProvider.CreateScope())
        {
            var workflow = scope.ServiceProvider.GetRequiredService<ShippingWorkflow>();
            var repository = scope.ServiceProvider.GetRequiredService<IShipmentRepository>();

            try
            {
                // 1. Convertim Mesajul în Comandă Internă
                var command = new ProcessShipmentCommand
                {
                    OrderId = invoiceMsg.OrderId,
                    CustomerId = invoiceMsg.CustomerId,
                    ShippingAddress = invoiceMsg.ShippingAddress,
                    Lines = invoiceMsg.Lines
                };

                // 2. Executăm Workflow-ul (Validare -> Calcul -> Generare AWB)
                IShipmentManifestedEvent manifestedEvent = workflow.Execute(command);

                // 3. Salvăm în Baza de Date
                await repository.SaveAsync(manifestedEvent);

                // 4. Pregătim Mesajul de Ieșire (ShipmentManifestedMessage)
                var outputMessage = new ShipmentManifestedMessage(
                    OrderId: manifestedEvent.OrderId,
                    CustomerId: manifestedEvent.CustomerId,
                    Awb: manifestedEvent.Awb,
                    ShippingAddress: manifestedEvent.ShippingAddress,
                    Lines: manifestedEvent.Lines,
                    ShippingCost: manifestedEvent.ShippingCost, // Aici e decimal sau Money.Amount
                    DispatchedAt: manifestedEvent.ManifestedAt
                );

                // 5. Trimitem notificarea în lume
                var body = BinaryData.FromObjectAsJson(outputMessage);
                await _shippingSender.SendMessageAsync(new ServiceBusMessage(body));

                // 6. Confirmăm succesul (Șterge mesajul din coadă)
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare procesare livrare: {ex.Message}");
                // Opțional: Putem trimite la DeadLetter dacă e eroare permanentă
                // await args.DeadLetterMessageAsync(args.Message, "ProcessingError", ex.Message);
                // Sau lăsăm să se reîncerce automat (nu dăm Complete)
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync(cancellationToken);
        await _processor.DisposeAsync();
        await _shippingSender.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}