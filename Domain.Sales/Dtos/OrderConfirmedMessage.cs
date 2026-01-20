using System;

namespace Domain.Sales.Dtos
{
    public record OrderConfirmedMessage
    {
        public Guid OrderId { get; init; }
        public decimal TotalAmount { get; init; }
        public string Currency { get; init; }
        public DateTime Date { get; init; }
        public string CustomerAddress { get; init; } 
    }
}