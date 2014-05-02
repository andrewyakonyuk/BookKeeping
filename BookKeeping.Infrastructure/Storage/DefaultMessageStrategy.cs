using BookKeeping.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Infrastructure.Storage
{
    public sealed class DefaultMessageStrategy : IMessageStrategy
    {
        public void Serialize<TEntity>(TEntity entity, System.IO.Stream stream)
        {
            ProtoBuf.Serializer.Serialize(stream, entity);
        }

        public TEntity Deserialize<TEntity>(System.IO.Stream stream)
        {
            return ProtoBuf.Serializer.Deserialize<TEntity>(stream);
        }
    }
}
