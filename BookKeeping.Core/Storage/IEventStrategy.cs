using System.IO;

namespace BookKeeping.Core.Storage
{
    public interface IEventStrategy
    {
        void Serialize(IEvent[] entity, Stream stream);

        IEvent[] Deserialize(Stream stream);
    }
}