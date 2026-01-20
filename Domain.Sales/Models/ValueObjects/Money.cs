using Domain.Sales.Exceptions;

namespace Domain.Sales.Models.ValueObjects
{
    public record Money
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency = "RON")
        {
            // Putem accepta 0, dar nu valori negative
            if (amount < 0) 
                throw new InvalidMoneyException($"Amount cannot be negative: {amount}");
            
            Amount = amount;
            Currency = currency;
        }

        // Adunare: Money + Money
        public static Money operator +(Money a, Money b)
        {
            if (a.Currency != b.Currency) 
                throw new InvalidMoneyException($"Cannot add different currencies: {a.Currency} vs {b.Currency}");
            
            return new Money(a.Amount + b.Amount, a.Currency);
        }

        // Înmulțire: Money * int (ex: Preț * Cantitate)
        public static Money operator *(Money a, int b) => new Money(a.Amount * b, a.Currency);
        
        // Înmulțire: Money * decimal
        public static Money operator *(Money a, decimal b) => new Money(a.Amount * b, a.Currency);

        public static Money Zero(string currency = "RON") => new Money(0, currency);

        public override string ToString() => $"{Amount:0.00} {Currency}";
    }
}