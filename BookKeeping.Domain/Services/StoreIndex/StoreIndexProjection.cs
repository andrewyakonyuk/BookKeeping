using BookKeeping.Core.AtomicStorage;
using BookKeeping.Core.Domain;
using BookKeeping.Domain.Contracts.Product.Events;
using BookKeeping.Domain.Contracts.Store.Events;

namespace BookKeeping.Domain.Services.StoreIndex
{
    public sealed class StoreIndexProjection :
        IEventHandler<StoreCreated>,
        IEventHandler<ProductCreated>,
        IEventHandler<StoreClosed>
    {
        readonly IDocumentWriter<string, StoreIndexView> _writer;

        public StoreIndexProjection(IDocumentWriter<string, StoreIndexView> writer)
        {
            _writer = writer;
        }

        public void When(StoreCreated e)
        {
            _writer.Add(e.Id.ToString(), new StoreIndexView());
        }

        public void When(ProductCreated e)
        {
            _writer.UpdateEnforcingNew(e.Store.ToString(), v =>
            {
                v.Products.Add(new ProductIndexView
                {
                    Id = e.Id
                });
            });
        }

        public void When(StoreClosed e)
        {
            _writer.TryDelete(e.Id.ToString());
        }
    }
}
