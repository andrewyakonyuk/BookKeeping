using BookKeeping.Domain.Contracts;
using BookKeeping.Persistance.Storage;
using System.Collections.Generic;
using System.Linq;

namespace BookKeeping.Domain
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        readonly IEventStore _eventStore;
        readonly IEventBus _eventBus;
        readonly IDictionary<IIdentity, AggregateBase> _identityMap = new Dictionary<IIdentity, AggregateBase>();
        bool _isCommit = false;

        public UnitOfWork(IEventStore eventStore, IEventBus eventBus)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
        }

        public void RegisterForTracking<TAggregate>(TAggregate aggregateRoot, IIdentity id)
             where TAggregate : AggregateBase
        {
            if (!_identityMap.ContainsKey(id))
            {
                _identityMap.Add(id, aggregateRoot);
            }
        }

        public TAggregate Get<TAggregate>(IIdentity id)
            where TAggregate : AggregateBase
        {
            if (_identityMap.ContainsKey(id))
            {
                return (TAggregate)_identityMap[id];
            }
            return null;
        }

        public void Commit()
        {
            foreach (var item in _identityMap.Select(t => t.Value).Where(t => t.Changes.Any()))
            {
                while (true)
                {
                    var eventStream = _eventStore.LoadEventStream(((dynamic)item).Id);
                    try
                    {
                        _eventStore.AppendToStream(((dynamic)item).Id, eventStream.Version, item.Changes);
                        foreach (var @event in item.Changes)
                        {
                            var realEvent = (dynamic)System.Convert.ChangeType(@event, @event.GetType());
                            _eventBus.Publish(realEvent);
                        }
                        break;
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        foreach (var clientEvent in item.Changes)
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
                        _eventStore.AppendToStream(((dynamic)item).Id, ex.ActualVersion, item.Changes);
                        foreach (var @event in item.Changes)
                        {
                            var realEvent = (dynamic)System.Convert.ChangeType(@event, @event.GetType());
                            _eventBus.Publish(realEvent);
                        }
                    }
                }
            }
            _identityMap.Clear();
            _isCommit = true;
        }

        static bool ConflictsWith(IEvent x, IEvent y)
        {
            return x.GetType() == y.GetType();
        }

        public void Rollback()
        {
            //todo:
            _identityMap.Clear();
        }

        public void Dispose()
        {
            if (!_isCommit)
            {
                Rollback();
            }
        }
    }
}
