using System;

namespace Domain.Sales.Exceptions
{
    public class InvalidAddressException : Exception
    {
        public InvalidAddressException(string message) : base(message) { }
    }
}