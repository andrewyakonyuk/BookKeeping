using BookKeeping.Core.Domain;
using BookKeeping.Domain.Contracts.Product;
using BookKeeping.Domain.Contracts.Store;
using BookKeeping.Domain.Contracts.Store.Events;
using System.Collections.Generic;

namespace BookKeeping.Domain.Aggregates.Store
{
    public sealed class StoreState
    {
        public StoreState(IEnumerable<IEvent> events)
        {
            foreach (var e in events)
            {
                Mutate(e);
            }
        }

        public void Mutate(IEvent e)
        {
            ((dynamic)this).When((dynamic)e);
        }

        public StoreId Id { get; private set; }

        public string Name { get; private set; }

        public bool IsClosed { get; private set; }

        public List<ProductId> Products { get; private set; }

        public void When(StoreCreated e)
        {
            Id = e.Id;
            Name = e.Name;
        }

        public void When(StoreRenamed e)
        {
            Name = e.NewName;
        }

        public void When(StoreClosed e)
        {
            IsClosed = true;
        }
    }
}
