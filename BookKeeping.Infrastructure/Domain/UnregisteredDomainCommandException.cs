using System;

namespace BookKeeping.Infrastructure.Domain
{
    public class UnregisteredDomainCommandException : Exception
    {
        public UnregisteredDomainCommandException(string message)
            : base(message)
        {
        }
    }
}