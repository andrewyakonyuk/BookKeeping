using BookKeeping.Core;
using BookKeeping.Core.AtomicStorage;
using BookKeeping.Core.Domain;
using BookKeeping.Domain.Contracts;
using System.Linq;

namespace BookKeeping.Projections.ProductsList
{
    public class ProductsProjection : 
        IEventHandler<ProductCreated>,
        IEventHandler<ProductStockUpdated>,
        IEventHandler<ProductRenamed>,
        IEventHandler<ProductItemNoChanged>,
        IEventHandler<ProductBarcodeChanged>,
        IEventHandler<ProductPriceChanged>,
        IEventHandler<ProductUOMChanged>,
        IEventHandler<ProductVATChanged>
    {
        private readonly IDocumentWriter<unit, ProductListView> _store;

        public ProductsProjection(IDocumentWriter<unit, ProductListView> store)
        {
            _store = store;
        }

        public void When(ProductCreated e)
        {
            _store.UpdateEnforcingNew(unit.it, prs => prs.Products.Add(
            new ProductView
            {
                Id = e.Id,
                Title = e.Title,
                ItemNo = e.ItemNo,
                Price = e.Price,
                Stock = e.Stock
            }));
        }

        public void When(ProductStockUpdated e)
        {
            _store.UpdateOrThrow(unit.it, list => list.Products.Single(p => p.Id == e.Id).Stock += e.Quantity);
        }

        public void When(ProductRenamed e)
        {
            _store.UpdateOrThrow(unit.it, list => list.Products.Single(p => p.Id == e.Id).Title = e.NewTitle);
        }

        public void When(ProductItemNoChanged e)
        {
            _store.UpdateOrThrow(unit.it, list => list.Products.Single(p => p.Id == e.Id).ItemNo = e.NewItemNo);
        }

        public void When(ProductBarcodeChanged e)
        {
            _store.UpdateOrThrow(unit.it, list => list.Products.Single(p => p.Id == e.Id).Barcode = e.NewBarcode);
        }

        public void When(ProductPriceChanged e)
        {
            _store.UpdateOrThrow(unit.it, list => list.Products.Single(p => p.Id == e.Id).Price = e.NewPrice);
        }

        public void When(ProductUOMChanged e)
        {
            _store.UpdateOrThrow(unit.it, list => list.Products.Single(p => p.Id == e.Id).UOM = e.NewUOM);
        }

        public void When(ProductVATChanged e)
        {
            _store.UpdateOrThrow(unit.it, list => list.Products.Single(p => p.Id == e.Id).VAT = e.NewVAT);
        }
    }
}
