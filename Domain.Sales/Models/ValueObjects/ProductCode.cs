using System.Text.RegularExpressions;
using Domain.Sales.Exceptions;

namespace Domain.Sales.Models.ValueObjects
{
    public record ProductCode
    {
        private static readonly Regex Pattern = new("^PROD-[0-9]{4}$"); // Format strict

        public string Value { get; }

        public ProductCode(string value)
        {
            if (Pattern.IsMatch(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidProductCodeException(value);
            }
        }

        public static bool TryParse(string value, out ProductCode? productCode)
        {
            if (Pattern.IsMatch(value))
            {
                productCode = new ProductCode(value);
                return true;
            }
            productCode = null;
            return false;
        }

        public override string ToString() => Value;
    }
}