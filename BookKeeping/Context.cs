using BookKeeping.Core;
using BookKeeping.Core.AtomicStorage;
using BookKeeping.Core.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BookKeeping
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

            var documentStrategy = new DefaultDocumentStrategy();
            EventStore = new EventStore(appendOnlyStore);
            ViewDocs = new FileDocumentStore(pathToStore, documentStrategy);

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
