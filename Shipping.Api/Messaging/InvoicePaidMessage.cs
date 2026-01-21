using Domain.Shipping.Models;

namespace Shipping.Api.Messaging;

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