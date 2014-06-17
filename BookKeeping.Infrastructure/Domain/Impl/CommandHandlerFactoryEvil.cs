using BookKeeping.Domain.Contracts;
using BookKeeping.Persistance.AtomicStorage;
using BookKeeping.Persistance.Storage;
using System;
using System.Linq;

namespace BookKeeping.Domain
{
    public sealed class CommandHandlerFactoryEvil : ICommandHandlerFactory
    {
        private readonly IDocumentStore _documentStore;
        private readonly IEventBus _eventBus;
        private readonly IEventStore _eventStore;
        private readonly IUnitOfWork _unitOfWork;
        private static ICommandHandlerProvider[] _handlerProviders;

        static CommandHandlerFactoryEvil()
        {
            _handlerProviders = AppDomain.CurrentDomain.GetAssemblies().Where(t => t.FullName.StartsWith("BookKeeping"))
                 .SelectMany(t => t.GetExportedTypes())
                 .Where(t => typeof(ICommandHandlerProvider).IsAssignableFrom(t) && !t.IsAbstract)
                 .Select(t => (ICommandHandlerProvider)Activator.CreateInstance(t))
                 .ToArray();
        }

        public CommandHandlerFactoryEvil(IDocumentStore documentStore, IUnitOfWork unitOfWork, IEventStore eventStore, IEventBus eventBus)
        {
            _documentStore = documentStore;
            _eventBus = eventBus;
            _eventStore = eventStore;
            _unitOfWork = unitOfWork;
        }

        public ICommandHandler<T> GetHandler<T>()
            where T : ICommand
        {
            return (ICommandHandler<T>)_handlerProviders.SelectMany(t => t.EntityApplicationServices(_documentStore, _eventStore, _eventBus, _unitOfWork))
                .SingleOrDefault(service => service is ICommandHandler<T>);
        }
    }
}
