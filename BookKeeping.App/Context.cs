using BookKeeping.Core;
using BookKeeping.Core.AtomicStorage;
using BookKeeping.Core.Domain;
using BookKeeping.Core.Storage;
using BookKeeping.Infrastructure;
using BookKeeping.Infrastructure.AtomicStorage;
using BookKeeping.Infrastructure.Storage;
using System.IO;

namespace BookKeeping.App
{
    public sealed class Context
    {
        public IDocumentStore ViewDocs { get; private set; }
        public ICommandBus CommandBus { get; private set; }
        public IEventStore EventStore { get; private set; }

        static Context _this;
        static object _lock = new object();

        public Context()
        {
            //TODO: move to configuration
            var pathToStore = Path.Combine(Directory.GetCurrentDirectory(), "store");
            var appendOnlyStore = new FileAppendOnlyStore(pathToStore);
            appendOnlyStore.Initialize();

            EventStore = new EventStore(appendOnlyStore,new DefaultMessageStrategy());
            ViewDocs = new FileDocumentStore(pathToStore, new DefaultDocumentStrategy());

            var eventBus = new EventBus(new EventHandlerFactoryImpl(ViewDocs));
            CommandBus = new CommandBus(new CommandHandlerFactoryImpl(ViewDocs, EventStore, eventBus));
        }

        public static Context Current
        {
            get
            {
                lock (_lock)
                {
                    if (_this == null)
                    {
                        _this = new Context();
                    }
                    return _this;
                }
            }
        }
    }
}
