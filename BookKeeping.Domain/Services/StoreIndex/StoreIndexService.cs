using BookKeeping.Core.AtomicStorage;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Contracts.Product;
using BookKeeping.Domain.Contracts.Store;
using System.Linq;

namespace BookKeeping.Domain.Services.StoreIndex
{
    public sealed class StoreIndexService : IStoreIndexService
    {
        readonly IDocumentReader<string, StoreIndexView> _storage;

        public StoreIndexService(IDocumentReader<string, StoreIndexView> storage)
        {
            _storage = storage;
        }

        public bool IsProductRegistered(ProductId product, StoreId store)
        {
            return _storage.Get(store.ToString())
                .Convert(i => i.Products.Where(t => t.Id.Equals(product)).Any(), false);
        }

        public bool IsStoreRegistered(StoreId store)
        {
            return _storage.Get(store.ToString()).HasValue;
        }

        public StoreIndexView LoadStoreIndex(StoreId id)
        {
            return _storage.Load(id.ToString());
        }
    }

    public interface IStoreIndexService
    {
        bool IsProductRegistered(ProductId product, StoreId store);

        bool IsStoreRegistered(StoreId store);

        StoreIndexView LoadStoreIndex(StoreId id);
    }
}
