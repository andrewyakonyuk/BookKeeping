using System;

namespace BookKeeping.Domain
{
    [Serializable]
    public class InvalidDomainOperationException: Exception
    {
        public InvalidDomainOperationException()
        {
        }

        public InvalidDomainOperationException(string message)
            : base(message)
        {
        }

        public InvalidDomainOperationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected InvalidDomainOperationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}