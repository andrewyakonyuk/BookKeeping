using BookKeeping.Infrastructure.Dependency;
using System;

namespace BookKeeping.Infrastructure.Logging
{
    public class LoggingService : ILoggingService
    {
        private readonly ILoggingProvider _loggingProvider;

        public static ILoggingService Instance
        {
            get
            {
                return DependencyResolver.Current.GetService<ILoggingService>();
            }
        }

        public LoggingService(ILoggingProvider loggingProvider)
        {
            this._loggingProvider = loggingProvider;
        }

        public void Log(Exception exception)
        {
            this._loggingProvider.Log(exception);
        }

        public void Log(string message)
        {
            this._loggingProvider.Log(message);
        }

        public void Log(Exception exception, string message)
        {
            this._loggingProvider.Log(exception, message);
        }
    }
}
