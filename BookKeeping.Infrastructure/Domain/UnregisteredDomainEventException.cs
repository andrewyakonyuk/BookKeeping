using System;

namespace BookKeeping.Domain
{
    public class UnregisteredDomainEventException : Exception
    {
        public UnregisteredDomainEventException(string message)
            : base(message)
        {
        }
    }
}