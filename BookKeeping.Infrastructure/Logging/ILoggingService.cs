using System;

namespace BookKeeping.Infrastructure.Logging
{
    public interface ILoggingService
    {
        void Log(Exception exception);

        void Log(string message);

        void Log(Exception exception, string message);
    }
}
