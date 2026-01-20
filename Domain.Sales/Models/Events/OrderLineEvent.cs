namespace Domain.Sales.Models.Events
{
    public class OrderLineEvent
    {
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
        public MoneyEvent UnitPrice { get; set; }
    }
}