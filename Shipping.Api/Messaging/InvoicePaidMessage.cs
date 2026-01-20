using Domain.Shipping.Models;

namespace Shipping.Api.Messaging;

// Acesta este mesajul pe care îl primești (Consumi) de la RabbitMQ/ServiceBus.
// Semnalează că totul e plătit și marfa poate fi expediată.
public record InvoicePaidMessage(
    Guid OrderId,
    Guid CustomerId,
    
    // Adresa unde trebuie să ajungă coletul
    ShippingAddress ShippingAddress,
    
    // Lista de produse de împachetat
    IReadOnlyCollection<ShipmentLine> Lines,
    
    // Valoarea declarată (pentru asigurare colet) - opțional
    Money InsuredValue,           
    
    // Data plății (momentul din care curge termenul de livrare)
    DateTime PaidDate      
);