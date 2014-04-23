using BookKeeping.Core;
using BookKeeping.Core.AtomicStorage;
using BookKeeping.Domain;
using BookKeeping.Domain.CustomerAggregate;
using BookKeeping.Domain.ProductAggregate;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace BookKeeping.Projections
{
    [DataContract]
    [Serializable]
    public class ProductDto : NotificationObject
    {
        ProductId _id;
        string _title = string.Empty;
        string _barcode = string.Empty;
        string _itemNo = string.Empty;
        CurrencyAmount _price;
        double _stock;
        string _uom;
        double _vat;

        public ProductId Id
        {
            get
            {
                return _id;
            }
            set
            {
                OnPropertyChanging(() => Id);
                _id = value;
                OnPropertyChanged(() => Id);
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                OnPropertyChanging(() => Title);
                _title = value;
                OnPropertyChanged(() => Title);
            }
        }

        public string Barcode
        {
            get
            {
                return _barcode;
            }
            set
            {
                OnPropertyChanging(() => Barcode);
                _barcode = value;
                OnPropertyChanged(() => Barcode);
            }
        }

        public string ItemNo
        {
            get
            {
                return _itemNo;
            }
            set
            {
                OnPropertyChanging(() => ItemNo);
                _itemNo = value;
                OnPropertyChanged(() => ItemNo);
            }
        }

        public CurrencyAmount Price
        {
            get
            {
                return _price;

            }
            set
            {
                OnPropertyChanging(() => Price);
                _price = value;
                OnPropertyChanged(() => Price);
            }
        }

        public double Stock
        {
            get
            {
                return _stock;
            }
            set
            {
                OnPropertyChanging(() => Stock);
                _stock = value;
                OnPropertyChanged(() => Stock);
            }
        }

        public string UOM
        {
            get
            {
                return _uom;
            }
            set
            {
                OnPropertyChanging(() => UOM);
                _uom = value;
                OnPropertyChanged(() => UOM);
            }
        }

        public double VAT
        {
            get
            {
                return _vat;
            }
            set
            {
                OnPropertyChanging(() => VAT);
                _vat = value;
                OnPropertyChanged(() => VAT);
            }
        }
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
