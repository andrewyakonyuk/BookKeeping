using System;

namespace BookKeeping.Infrastructure.Domain
{
    public class UnregisteredDomainEventException : Exception
    {
        public UnregisteredDomainEventException(string message)
            : base(message)
        {
        }
    }
}