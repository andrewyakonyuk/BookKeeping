using System;

namespace BookKeeping.Core
{
    public class UnregisteredDomainEventException : Exception
    {
        public UnregisteredDomainEventException(string message)
            : base(message)
        {
        }
    }
}