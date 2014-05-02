using BookKeeping.Core;
using BookKeeping.Core.AtomicStorage;
using BookKeeping.Core.Domain;
using BookKeeping.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace BookKeeping.Projections
{
    [DataContract]
    [Serializable]
    public class ProductDto
    {
        public ProductId Id { get; set; }

        public string Title { get; set; }

        public string Barcode { get; set; }

        public string ItemNo { get; set; }

        public CurrencyAmount Price { get; set; }

        public double Stock { get; set; }

        public string UOM { get; set; }

        public double VAT { get; set; }
    }

    [DataContract]
    [Serializable]
    public class ProductListDto
    {
        private IList<ProductDto> _products = new List<ProductDto>();

        public IList<ProductDto> Products { get { return _products; } }
    }

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
        private readonly IDocumentWriter<unit, ProductListDto> _store;

        public ProductsProjection(IDocumentWriter<unit, ProductListDto> store)
        {
            _store = store;
        }

        public void When(ProductCreated e)
        {
            _store.UpdateEnforcingNew(unit.it, prs => prs.Products.Add(
            new ProductDto
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
