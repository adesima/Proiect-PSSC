using Domain.Shipping.Models;

namespace Domain.Shipping.Models;

public record ManifestedShipment
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }

    // Identificatorul unic generat de tine (echivalentul InvoiceId)
    public required AwbCode Awb { get; init; }

    public required ShippingAddress ShippingAddress { get; init; }
    public IReadOnlyCollection<ShipmentLine> Lines { get; init; } = Array.Empty<ShipmentLine>();

    // Costul final al transportului (echivalentul Total de la factură)
    public required Money ShippingCost { get; init; }

    public DateTime ManifestedAt { get; init; }

    // Metoda care transformă starea internă în evenimentul public
    public IShipmentManifestedEvent ToEvent() =>
        new ShipmentManifestedEvent(
            OrderId: OrderId,
            CustomerId: CustomerId,
            ShippingAddress: ShippingAddress,
            Lines: Lines,
            ShippingCost: ShippingCost,
            Awb: Awb,
            ManifestedAt: ManifestedAt);
}