﻿using BookKeeping.Persistance.AtomicStorage;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BookKeeping.Persistance.AtomicStorage
{
    public sealed class DefaultDocumentStrategy : IDocumentStrategy
    {
        public TEntity Deserialize<TEntity>(Stream stream)
        {
            var signature = stream.ReadByte();

            if (signature != 42)
                throw new InvalidOperationException("Unknown view format");

            return ProtoBuf.Serializer.Deserialize<TEntity>(stream);
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
            stream.WriteByte(42);
            ProtoBuf.Serializer.Serialize(stream, entity);
        }
    }
}