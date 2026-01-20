namespace Domain.Sales.Models.Events
{
    public class AddressEvent
    {
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
    }
}