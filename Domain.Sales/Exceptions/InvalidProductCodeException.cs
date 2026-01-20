using System;

namespace Domain.Sales.Exceptions
{
    public class InvalidProductCodeException : Exception
    {
        public InvalidProductCodeException(string message) : base(message) { }
    }
}