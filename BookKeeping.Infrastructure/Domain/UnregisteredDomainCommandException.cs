using System;

namespace BookKeeping.Domain
{
    public class UnregisteredDomainCommandException : Exception
    {
        public UnregisteredDomainCommandException(string message)
            : base(message)
        {
        }
    }
}