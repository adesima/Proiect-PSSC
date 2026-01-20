using Domain.Sales.Exceptions;

namespace Domain.Sales.Models.ValueObjects
{
    public record Quantity
    {
        public int Value { get; }

        public Quantity(int value)
        {
            if (value > 0 && value <= 100) // Limită de business
            {
                Value = value;
            }
            else
            {
                throw new InvalidQuantityException($"{value} is not a valid quantity. Must be between 1 and 100.");
            }
        }

        // Permite adunarea a două cantități direct
        public static Quantity operator +(Quantity a, Quantity b) => new Quantity(a.Value + b.Value);

        public override string ToString() => Value.ToString();
    }
}