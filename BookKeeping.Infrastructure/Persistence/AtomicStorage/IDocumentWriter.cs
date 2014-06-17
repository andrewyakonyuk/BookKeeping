using System;

namespace BookKeeping.Persistance.AtomicStorage
{
    /// <summary>
    /// This is a sample of strongly-typed document writer interface,
    /// which works good for building simple systems that can be migrated
    /// between cloud and various on-premises implementations.
    /// This interface supports automated view rebuilding
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IDocumentWriter<in TKey, TEntity>
    {
        TEntity AddOrUpdate(TKey key, Func<TEntity> addFactory, Func<TEntity, TEntity> update, AddOrUpdateHint hint = AddOrUpdateHint.ProbablyExists);

        bool TryDelete(TKey key);
    }

    public enum AddOrUpdateHint
    {
        ProbablyExists,
        ProbablyDoesNotExist
    }
}