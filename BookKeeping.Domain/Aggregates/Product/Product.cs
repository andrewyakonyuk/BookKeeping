using BookKeeping.Core.Domain;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Contracts.Product;
using BookKeeping.Domain.Contracts.Product.Events;
using BookKeeping.Domain.Contracts.Store;
using BookKeeping.Domain.Services.StoreIndex;
using System;
using System.Collections.Generic;

namespace BookKeeping.Domain.Aggregates.Product
{
    public class Product
    {
        public readonly IList<IEvent> _changes = new List<IEvent>();
        private readonly ProductState _state;

        public Product(IEnumerable<IEvent> events)
        {
            _state = new ProductState(events);
        }

        private void Apply(IEvent e)
        {
            _state.Mutate(e);
            _changes.Add(e);
        }

        public void Create(ProductId id, StoreId warehouse, string title, string itemNo, CurrencyAmount price,
            double stock, string unitOfMeasure, VatRate vatRate, IStoreIndexService warehouseService, DateTime utc)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("Title should be not null or empty", "title");
            if (price.Amount < 0)
                throw new ArgumentException("Price should be prositive", "price");

            if (warehouseService.IsProductRegistered(id, warehouse))
            {
                throw new ArgumentException("Id should be unique in the warehouse", "id");
            }

            Apply(new ProductCreated
            {
                Id = id,
                Title = title,
                ItemNo = itemNo,
                Price = price,
                Stock = stock,
                UnitOfMeasure = unitOfMeasure,
                Store = warehouse,
                VatRate = vatRate,
                Utc = utc
            });
        }

        public void UpdateStock(double quantity, string reason, DateTime utc)
        {
            Apply(new ProductStockUpdated
            {
                Id = _state.Id,
                Quantity = quantity,
                Reason = reason,
                Utc = utc
            });
        }

        public void Rename(string title, DateTime utc)
        {
            Apply(new ProductRenamed
            {
                Id = _state.Id,
                NewTitle = title,
                Utc = utc
            });
        }

        public void ChangeBarcode(string barcode, DateTime utc)
        {
            Apply(new ProductBarcodeChanged
            {
                Id = _state.Id,
                NewBarcode = barcode,
                Utc = utc
            });
        }

        public void ChangeItemNo(string itemNo, DateTime utc)
        {
            Apply(new ProductItemNoChanged
            {
                Id = _state.Id,
                NewItemNo = itemNo,
                Utc = utc
            });
        }

        public void ChangePrice(CurrencyAmount price, DateTime utc)
        {
            Apply(new ProductPriceChanged
            {
                Id = _state.Id,
                NewPrice = price,
                Utc = utc
            });
        }

        public void ChangeUnitOfMeasure(string unitOfMeasure, DateTime utc)
        {
            Apply(new ProductUnitOfMeasureChanged
            {
                Id = _state.Id,
                NewUnitOfMeasure = unitOfMeasure,
                Utc = utc
            });
        }

        public void ChangeVatRate(VatRate vat, DateTime utc)
        {
            Apply(new ProductVatRateChanged
            {
                Id = _state.Id,
                NewVatRate = vat,
                Utc = utc
            });
        }

        public IList<IEvent> Changes { get { return _changes; } }
    }
}
