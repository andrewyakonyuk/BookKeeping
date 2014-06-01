using BookKeeping.Domain.Contracts;
using BookKeeping.Infrastructure.Domain;
using BookKeeping.Persistent;
using BookKeeping.Persistent.AtomicStorage;
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
        IEventHandler<ProductUnitOfMeasureChanged>,
        IEventHandler<ProductVatRateChanged>,
        IEventHandler<ProductMakedOrderable>,
        IEventHandler<ProductMakedNonOrderable>
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
                Stock = e.Stock,
                UnitOfMeasure = e.UnitOfMeasure,
                VatRate = e.VatRate
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

        public void When(ProductUnitOfMeasureChanged e)
        {
            _store.UpdateOrThrow(unit.it, list => list.Products.Single(p => p.Id == e.Id).UnitOfMeasure = e.NewUnitOfMeasure);
        }

        public void When(ProductVatRateChanged e)
        {
            _store.UpdateOrThrow(unit.it, list => list.Products.Single(p => p.Id == e.Id).VatRate = e.NewVatRate);
        }

        public void When(ProductMakedOrderable e)
        {
            _store.UpdateOrThrow(unit.it, list => list.Products.Single(p => p.Id == e.Id).IsOrderable = true);
        }

        public void When(ProductMakedNonOrderable e)
        {
            _store.UpdateOrThrow(unit.it, list => list.Products.Single(p => p.Id == e.Id).IsOrderable = false);
        }
    }
}
