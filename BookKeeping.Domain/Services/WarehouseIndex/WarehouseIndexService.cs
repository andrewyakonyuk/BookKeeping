using BookKeeping.Core.AtomicStorage;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Contracts.Product;
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

        public bool IsSkuRegistered(ProductId sku, WarehouseId warehouse)
        {
            return _storage.Get(warehouse.ToString())
                .Convert(i => i.Skus.Where(t => t.Id.Equals(sku)).Any(), false);
        }

        public bool IsWarehouseRegistered(WarehouseId warehouse)
        {
            return _storage.Get(warehouse.ToString()).HasValue;
        }

        public WarehouseIndexView LoadWarehouseIndex(WarehouseId id)
        {
            return _storage.Load(id.ToString());
        }
    }

    public interface IWarehouseIndexService
    {
        bool IsSkuRegistered(ProductId sku, WarehouseId warehouse);

        bool IsWarehouseRegistered(WarehouseId warehouse);

        WarehouseIndexView LoadWarehouseIndex(WarehouseId id);
    }
}
