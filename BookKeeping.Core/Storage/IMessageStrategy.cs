using System;
using System.IO;

namespace BookKeeping.Core.Storage
{
    public interface IMessageStrategy
    {
        void WriteMessage(object entity, Type type, Stream stream);

        object ReadMessage(Stream stream);

        int ReadCompactInt(Stream stream);

        void WriteCompactInt(int value, Stream stream);
    }
}
