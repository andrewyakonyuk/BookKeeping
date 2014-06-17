using System;
using System.IO;

namespace BookKeeping.Persistance.Storage
{
    public interface IMessageStrategy
    {
        void WriteMessage(object entity, Type type, Stream stream);

        object ReadMessage(Stream stream);

        int ReadCompactInt(Stream stream);

        void WriteCompactInt(int value, Stream stream);
    }
}
