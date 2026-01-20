namespace Domain.Sales.Models.Events
{
    public class MoneyEvent
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}