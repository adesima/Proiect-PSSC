using System;

namespace Domain.Sales.Exceptions
{
    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }
}