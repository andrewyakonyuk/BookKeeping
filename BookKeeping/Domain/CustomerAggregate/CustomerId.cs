using BookKeeping.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Domain.CustomerAggregate
{
    /// <summary>
    /// This is a customer identity. It is just a class that makes it explicit,
    /// that this specific <em>long</em> is not just any number, but an identifier
    /// of a customer aggregate. This has a lot of benefits in further development.
    /// </summary>
    [Serializable]
    public sealed class CustomerId : IdentityBase<long>, IIdentity
    {
        public const string Tag = "customer";

        public CustomerId(long id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return string.Format("customer-{0}", Id);
        }

        public override string GetTag()
        {
            return Tag;
        }
    }
}
