using System;

namespace BookKeeping.Core
{
    public class UnregisteredDomainCommandException : Exception
    {
        public UnregisteredDomainCommandException(string message)
            : base(message)
        {
        }
    }
}