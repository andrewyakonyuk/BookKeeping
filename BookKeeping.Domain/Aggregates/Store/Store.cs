using BookKeeping.Core.Domain;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Contracts.Store;
using BookKeeping.Domain.Contracts.Store.Events;
using BookKeeping.Domain.Services.StoreIndex;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookKeeping.Domain.Aggregates.Store
{
    public class Store
    {
        public readonly IList<IEvent> _changes = new List<IEvent>();
        private readonly StoreState _state;

        public Store(IEnumerable<IEvent> events)
        {
            _state = new StoreState(events);
        }

        private void Apply(IEvent e)
        {
            _state.Mutate(e);
            _changes.Add(e);
        }

        public IList<IEvent> Changes { get { return _changes; } }

        public void Create(StoreId id, string name, DateTime utc)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name should be not null or empty", "name");
            if (_state.Id != null)
                throw new InvalidOperationException("Store already created");

            Apply(new StoreCreated
            {
                Id = id,
                Name = name,
                Utc = utc
            });
        }

        public void Rename(string name, DateTime utc)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name should be not null or empty", "name");

            Apply(new StoreRenamed
            {
                Id = _state.Id,
                NewName = name,
                Utc = utc
            });
        }

        public void Close(string reason, IStoreIndexService warehouseIndex, DateTime utc)
        {
            if (warehouseIndex.LoadStoreIndex(_state.Id).Products.Any())
                throw new InvalidOperationException("All related products should be moved to another store");

            Apply(new StoreClosed
            {
                Id = _state.Id,
                Utc = utc,
                Reason = reason
            });
        }
    }
}
