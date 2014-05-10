using BookKeeping.Core.Domain;
using BookKeeping.Domain.Contracts;
using System.Collections.Generic;

namespace BookKeeping.Domain.Aggregates.Warehouse
{
    public sealed class WarehouseState
    {
        public WarehouseState(IEnumerable<IEvent> events)
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

        public WarehouseId Id { get; private set; }

        public string Name { get; private set; }

        public bool IsClosed { get; private set; }

        public List<SkuId> Skus { get; private set; }

        public void When(WarehouseCreated e)
        {
            Id = e.Id;
            Name = e.Name;
        }

        public void When(WarehouseRenamed e)
        {
            Name = e.NewName;
        }

        public void When(WarehouseClosed e)
        {
            IsClosed = true;
        }
    }
}
