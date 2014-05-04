using BookKeeping.Core.AtomicStorage;
using BookKeeping.Domain.Contracts;
using System.Linq;

namespace BookKeeping.Domain.Services.WarehouseIndex
{
    public sealed class WarehouseIndexService : IWarehouseIndexService
    {
        readonly IDocumentReader<string, WarehouseIndexView> _storage;

        public WarehouseIndexService(IDocumentReader<string, WarehouseIndexView> storage)
        {
            _storage = storage;
        }

        public bool IsSkuRegistered(SkuId sku, WarehouseId warehouse)
        {
            return _storage.Get(warehouse.ToString())
                .Convert(i => i.Skus.Where(t => t.Id.Equals(sku)).Any(), false);
        }
    }

    public interface IWarehouseIndexService
    {
        bool IsSkuRegistered(SkuId sku, WarehouseId warehouse);
    }
}
