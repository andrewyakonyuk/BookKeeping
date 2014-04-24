using System;

namespace BookKeeping.Infrastructure.Logging
{
    public interface ILoggingProvider
    {
        void Log(Exception exception);

        void Log(string message);

        void Log(Exception exception, string message);
    }
}
