using BookKeeping.Core;
using BookKeeping.Core.AtomicStorage;
using BookKeeping.Core.Domain;
using BookKeeping.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BookKeeping.Domain.Services.WarehouseIndex
{
    public sealed class WarehouseIndexProjection :
        IEventHandler<WarehouseCreated>,
        IEventHandler<ProductCreated>,
        IEventHandler<WarehouseClosed>
    {
        readonly IDocumentWriter<string, WarehouseIndexView> _writer;

        public WarehouseIndexProjection(IDocumentWriter<string, WarehouseIndexView> writer)
        {
            _writer = writer;
        }

        public void When(WarehouseCreated e)
        {
            _writer.Add(e.Id.ToString(), new WarehouseIndexView());
        }

        public void When(ProductCreated e)
        {
            _writer.UpdateEnforcingNew(e.Warehouse.ToString(), v =>
            {
                v.Skus.Add(new SkuIndexView
                {
                    Id = e.Id
                });
            });
        }

        public void When(WarehouseClosed e)
        {
            _writer.TryDelete(e.Id.ToString());
        }
    }
}
