using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;
using BookKeeping.Infrastructure.Domain;
using BookKeeping.Persistent.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Domain.Repositories
{
    public abstract class RepositoryBase<TAggregate, TKey> : IRepository<TAggregate, TKey>
        where TAggregate : AggregateBase
        where TKey : IIdentity
    {
        readonly IEventStore _eventStore;
        readonly IEventBus _eventBus;

        protected RepositoryBase(IEventStore eventStore, IEventBus eventBus)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
        }

        public virtual void Save(TAggregate product)
        {
            while (true)
            {
                var eventStream = _eventStore.LoadEventStream(((dynamic)product).Id);
                try
                {
                    _eventStore.AppendToStream(((dynamic)product).Id, eventStream.Version, product.Changes);
                    foreach (var @event in product.Changes)
                    {
                        var realEvent = (dynamic)System.Convert.ChangeType(@event, @event.GetType());
                        _eventBus.Publish(realEvent);
                    }
                    return;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    foreach (var clientEvent in product.Changes)
                    {
                        foreach (var actualEvent in ex.ActualEvents)
                        {
                            if (ConflictsWith(clientEvent, actualEvent))
                            {
                                throw new RealConcurrencyException(string.Format("Conflict between {0} and {1}", clientEvent, actualEvent), ex);
                            }
                        }
                    }
                    // there are no conflicts and we can append
                    _eventStore.AppendToStream(((dynamic)product).Id, ex.ActualVersion, product.Changes);
                    foreach (var @event in product.Changes)
                    {
                        var realEvent = (dynamic)System.Convert.ChangeType(@event, @event.GetType());
                        _eventBus.Publish(realEvent);
                    }
                }
            }
        }

        protected static bool ConflictsWith(IEvent x, IEvent y)
        {
            return x.GetType() == y.GetType();
        }

        public abstract IEnumerable<TAggregate> All();

        public abstract TAggregate Get(TKey id);

        public abstract TAggregate Load(TKey id);
    }
}
