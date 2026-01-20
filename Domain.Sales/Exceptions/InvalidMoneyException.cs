using System;

namespace Domain.Sales.Exceptions
{
    public class InvalidMoneyException : Exception
    {
        public InvalidMoneyException(string message) : base(message) { }
    }
}