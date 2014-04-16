using System.IO;

namespace BookKeeping.Core.AtomicStorage
{
    public interface IDocumentStrategy
    {
        string GetEntityBucket<TEntity>();

        string GetEntityLocation<TEntity>(object key);

        void Serialize<TEntity>(TEntity entity, Stream stream);

        TEntity Deserialize<TEntity>(Stream stream);
    }
}