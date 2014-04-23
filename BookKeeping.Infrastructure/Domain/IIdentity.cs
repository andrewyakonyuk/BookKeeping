using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Infrastructure.Domain
{
    public interface IIdentity
    {
        /// <summary>
        /// Gets the id, converted to a string. Only alphanumerics and '-' are allowed.
        /// </summary>
        /// <returns></returns>
        string GetId();

        /// <summary>
        /// Unique tag (should be unique within the assembly) to distinguish
        /// between different identities, while deserializing.
        /// </summary>
        string GetTag();

        int GetStableHashCode();
    }
}
