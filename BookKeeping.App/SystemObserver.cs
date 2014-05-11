using System;
using System.Diagnostics;
using System.Globalization;

namespace BookKeeping
{
    /// <summary>
    /// System event representing something that happened
    /// within the infrastructure
    /// </summary>
    public interface ISystemEvent
    {
    }

    public interface ISystemWarningEvent { }

    public static class SystemObserver
    {
        private static IObserver<ISystemEvent>[] _observers = new IObserver<ISystemEvent>[0];

        public static void Complete()
        {
            foreach (var observer in _observers)
            {
                observer.OnCompleted();
            }
        }

        public static void Notify(ISystemEvent @event)
        {
            foreach (var observer in _observers)
            {
                try
                {
                    observer.OnNext(@event);
                }
                catch (Exception ex)
                {
                    var message = string.Format("Observer {0} failed with {1}", observer, ex);
                    Trace.WriteLine(message);
                }
            }
        }

        public static void Notify(string message, params object[] args)
        {
            Notify(new MessageEvent(string.Format(CultureInfo.InvariantCulture, message, args)));
        }

        public static void Put(Action<ISystemEvent> se)
        {
            _observers = new IObserver<ISystemEvent>[] { new ActionObserver(se), };
        }

        public static IObserver<ISystemEvent>[] Swap(params IObserver<ISystemEvent>[] swap)
        {
            var old = _observers;
            _observers = swap;
            return old;
        }

        public sealed class MessageEvent : ISystemEvent
        {
            public readonly string Message;

            public MessageEvent(string message)
            {
                Message = message;
            }

            public override string ToString()
            {
                return Message;
            }
        }

        private sealed class ActionObserver : IObserver<ISystemEvent>
        {
            private readonly Action<ISystemEvent> _action;

            public ActionObserver(Action<ISystemEvent> action)
            {
                _action = action;
            }

            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
            }

            public void OnNext(ISystemEvent value)
            {
                _action(value);
            }
        }
    }
}