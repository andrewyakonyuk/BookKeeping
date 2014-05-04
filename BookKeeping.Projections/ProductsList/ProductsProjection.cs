using BookKeeping.Core;
using BookKeeping.Core.AtomicStorage;
using BookKeeping.Core.Domain;
using BookKeeping.Domain.Contracts;
using System.Linq;

namespace BookKeeping.Projections.ProductsList
{
    public class ProductsProjection : 
        IEventHandler<SkuCreated>,
        IEventHandler<SkuStockUpdated>,
        IEventHandler<SkuRenamed>,
        IEventHandler<SkuItemNoChanged>,
        IEventHandler<SkuBarcodeChanged>,
        IEventHandler<SkuPriceChanged>,
        IEventHandler<SkuUnitOfMeasureChanged>,
        IEventHandler<SkuVatRateChanged>
    {
        private readonly IDocumentWriter<unit, ProductListView> _store;

        public ProductsProjection(IDocumentWriter<unit, ProductListView> store)
        {
            _store = store;
        }

        public void When(SkuCreated e)
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
                VatRate = e.VatRate,
                Warehouse = e.Warehouse
            }));
        }

        public void When(SkuStockUpdated e)
        {
            _store.UpdateOrThrow(unit.it, list => list.Products.Single(p => p.Id == e.Id).Stock += e.Quantity);
        }

        public void When(SkuRenamed e)
        {
            _store.UpdateOrThrow(unit.it, list => list.Products.Single(p => p.Id == e.Id).Title = e.NewTitle);
        }

        public void When(SkuItemNoChanged e)
        {
            _store.UpdateOrThrow(unit.it, list => list.Products.Single(p => p.Id == e.Id).ItemNo = e.NewItemNo);
        }

        public void When(SkuBarcodeChanged e)
        {
            _store.UpdateOrThrow(unit.it, list => list.Products.Single(p => p.Id == e.Id).Barcode = e.NewBarcode);
        }

        public void When(SkuPriceChanged e)
        {
            _store.UpdateOrThrow(unit.it, list => list.Products.Single(p => p.Id == e.Id).Price = e.NewPrice);
        }

        public void When(SkuUnitOfMeasureChanged e)
        {
            _store.UpdateOrThrow(unit.it, list => list.Products.Single(p => p.Id == e.Id).UnitOfMeasure = e.NewUnitOfMeasure);
        }

        public void When(SkuVatRateChanged e)
        {
            _store.UpdateOrThrow(unit.it, list => list.Products.Single(p => p.Id == e.Id).VatRate = e.NewVatRate);
        }
    }
}
