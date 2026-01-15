using System;

namespace Domain.Shipping.Models
{
    public record AwbCode
    {
        public string Value { get; }

        private AwbCode(string value)
        {
            Value = value;
        }

        public static AwbCode Create(string value)
        {
            // Validare simplă: nu poate fi gol
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("AWB code cannot be empty.", nameof(value));
            }
            
            // Validari ...

            return new AwbCode(value);
        }

        public override string ToString() => Value;
    }
}