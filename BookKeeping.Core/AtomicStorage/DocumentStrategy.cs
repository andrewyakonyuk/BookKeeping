using ProtoBuf;
using System;
using System.IO;

namespace BookKeeping.Core.AtomicStorage
{
    public sealed class DocumentStrategy : IDocumentStrategy
    {
        public TEntity Deserialize<TEntity>(Stream stream)
        {
            var signature = stream.ReadByte();

            if (signature != 42)
                throw new InvalidOperationException("Unknown view format");

            return Serializer.Deserialize<TEntity>(stream);
        }

        public string GetEntityBucket<TEntity>()
        {
            return "bookkeeping-doc" + "/" + NameCache<TEntity>.Name;
        }

        public string GetEntityLocation<TEntity>(object key)
        {
            if (key is unit)
                return NameCache<TEntity>.Name + ".pb";

            return key.ToString().ToLowerInvariant() + ".pb";
        }

        public void Serialize<TEntity>(TEntity entity, Stream stream)
        {
            // ProtoBuf must have non-zero files
            stream.WriteByte(42);
            Serializer.Serialize(stream, entity);
        }
    }
}