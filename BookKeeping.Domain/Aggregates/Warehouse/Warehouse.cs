using BookKeeping.Core.Domain;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Services.WarehouseIndex;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookKeeping.Domain.Aggregates.Warehouse
{
    public class Warehouse
    {
        public readonly IList<IEvent> _changes = new List<IEvent>();
        private readonly WarehouseState _state;

        public Warehouse(IEnumerable<IEvent> events)
        {
            _state = new WarehouseState(events);
        }

        private void Apply(IEvent e)
        {
            _state.Mutate(e);
            _changes.Add(e);
        }

        public IList<IEvent> Changes { get { return _changes; } }

        public void Create(WarehouseId id, string name, DateTime utc)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name should be not null or empty", "name");
            if (_state.Id != null)
                throw new InvalidOperationException("Warehouse already created");

            Apply(new WarehouseCreated
            {
                Id = id,
                Name = name,
                Created = utc
            });
        }

        public void Rename(string name, DateTime utc)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name should be not null or empty", "name");

            Apply(new WarehouseRenamed
            {
                Id = _state.Id,
                NewName = name,
                Renamed = utc
            });
        }

        public void Close(string reason, IWarehouseIndexService warehouseIndex, DateTime utc)
        {
            if (warehouseIndex.LoadWarehouseIndex(_state.Id).Skus.Any())
                throw new InvalidOperationException("All related skus should be moved to another warehouse");

            Apply(new WarehouseClosed
            {
                Id = _state.Id,
                Closed = utc,
                Reason = reason
            });
        }

        public void AddSku(SkuId sku, DateTime utc)
        {

        }
    }
}
