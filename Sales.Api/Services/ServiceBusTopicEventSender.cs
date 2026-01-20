using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Domain.Sales.Services;
using Microsoft.Extensions.Configuration;

namespace Sales.Api.Services
{
    public class ServiceBusTopicEventSender : IEventSender
    {
        private readonly ServiceBusClient _client;

        public ServiceBusTopicEventSender(IConfiguration configuration)
        {
            // Citim connection string-ul din appsettings.json
            var connectionString = configuration.GetConnectionString("ServiceBus");
            _client = new ServiceBusClient(connectionString);
        }

        public async Task SendAsync<T>(string topicName, T message)
        {
            var sender = _client.CreateSender(topicName);
            
            var jsonMessage = JsonSerializer.Serialize(message);
            var serviceBusMessage = new ServiceBusMessage(jsonMessage)
            {
                ContentType = "application/json"
            };

            await sender.SendMessageAsync(serviceBusMessage);
        }
    }
}