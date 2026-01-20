using System.Threading.Tasks;

namespace Domain.Sales.Services
{
    public interface IEventSender
    {
        // Trimite un mesaj generic către un topic specific 
        Task SendAsync<T>(string topicName, T message);
    }
}