﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Persistent.AtomicStorage
{
    public interface IDocumentStore
    {
        IDocumentWriter<TKey, TEntity> GetWriter<TKey, TEntity>();
        IDocumentReader<TKey, TEntity> GetReader<TKey, TEntity>();
        IDocumentStrategy Strategy { get; }
        IEnumerable<DocumentRecord> EnumerateContents(string bucket);
        void WriteContents(string bucket, IEnumerable<DocumentRecord> records);
        void Reset(string bucket);
    }

    public sealed class DocumentRecord
    {
        /// <summary>
        /// Path of the view in the subfolder, using '/' as split on all platforms
        /// </summary>
        public readonly string Key;

        public readonly Func<byte[]> Read;

        public DocumentRecord(string key, Func<byte[]> read)
        {
            Key = key;
            Read = read;
        }
    }
}
