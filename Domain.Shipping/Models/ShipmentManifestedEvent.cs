// namespace Domain.Shipping.Models;
//
// public record ShipmentManifestedEvent
// {
//     // AWB-ul este identificatorul unic al livrării (echivalentul InvoiceId)
//     public required string Awb { get; init; }
//     
//     public Guid OrderId { get; init; }
//     
//     // Costul transportului
//     public required Money ShippingCost { get; init; }
//     
//     // Data la care s-a generat AWB-ul
//     public DateTime ManifestedAt { get; init; }
// }