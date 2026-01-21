using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Domain.Shipping.Models;
using Domain.Shipping.Repositories;
using Domain.Shipping.Workflows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shipping.Api.Messaging;

namespace Shipping.Api.BackgroundServices;

public class InvoicePaidListener : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceProvider _serviceProvider;

    public InvoicePaidListener(
        ServiceBusClient client,
        IConfiguration config,
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        // Configurare Input
        // Ascult topicul unde primesc mesajele 
        var billingTopic = config["ServiceBus:BillingTopic"];
        var shippingSubscription = config["ServiceBus:ShippingSubscription"];
        
        _processor = client.CreateProcessor(billingTopic, shippingSubscription, new ServiceBusProcessorOptions
        {
            // Sa nu proceseze prea multe deodata
            MaxConcurrentCalls = 1,
            AutoCompleteMessages = false
        });
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
        // Deserializare Mesaj Intrare (InvoicePaidMessage)
        var bodyJson = args.Message.Body.ToString();
        var invoiceMsg = JsonSerializer.Deserialize<InvoicePaidMessage>(bodyJson);

        if (invoiceMsg is null)
        {
            await args.DeadLetterMessageAsync(args.Message, "DeserializationFailed", "Body was null");
            return;
        }

        // Creare Scope (Pentru servicii Scoped: DB, Workflow)
        using (var scope = _serviceProvider.CreateScope())
        {
            var workflow = scope.ServiceProvider.GetRequiredService<ShippingWorkflow>();
            var repository = scope.ServiceProvider.GetRequiredService<IShipmentRepository>();

            try
            {
                // Convertesc mesajul in comanda interna
                var command = new ProcessShipmentCommand
                {
                    OrderId = invoiceMsg.OrderId,
                    CustomerId = invoiceMsg.CustomerId,
                    ShippingAddress = invoiceMsg.ShippingAddress,
                    Lines = invoiceMsg.Lines
                };

                // Execut Workflow-ul (Validare -> Calcul -> Generare AWB)
                IShipmentManifestedEvent manifestedEvent = workflow.Execute(command);

                // Salvez în baza de date
                await repository.SaveAsync(manifestedEvent); 
                
                // Șterge mesajul din coada
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare procesare livrare: {ex.Message}");
                
                // Trimit mesajele eronate in DeadLetter
                await args.DeadLetterMessageAsync(
                    args.Message, 
                    "ProcessingError", 
                    ex.Message);
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync(cancellationToken);
        await _processor.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}