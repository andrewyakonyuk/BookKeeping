using System.IO;

namespace BookKeeping.Core.Storage
{
    public interface IMessageStrategy
    {
        void Serialize<TEntity>(TEntity entity, Stream stream);

        TEntity Deserialize<TEntity>(Stream stream);
    }
}
