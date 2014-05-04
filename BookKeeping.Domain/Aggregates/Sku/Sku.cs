using BookKeeping.Core.Domain;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Services.WarehouseIndex;
using System;
using System.Collections.Generic;

namespace BookKeeping.Domain.Aggregates.Sku
{
    public class Sku
    {
        public readonly IList<IEvent> _changes = new List<IEvent>();
        private readonly SkuState _state;

        public Sku(IEnumerable<IEvent> events)
        {
            _state = new SkuState(events);
        }

        private void Apply(IEvent e)
        {
            _state.Mutate(e);
            _changes.Add(e);
        }

        public void Create(SkuId id, WarehouseId warehouse, string title, string itemNo, CurrencyAmount price,
            double stock, string unitOfMeasure, VatRate vatRate, IWarehouseIndexService warehouseService, DateTime utc)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("Title should be not null or empty", "title");
            if (price.Amount < 0)
                throw new ArgumentException("Price should be prositive", "price");

            if (warehouseService.IsSkuRegistered(id, warehouse))
            {
                throw new ArgumentException("Id should be unique in the warehouse", "id");
            }

            Apply(new SkuCreated
            {
                Id = id,
                Title = title,
                ItemNo = itemNo,
                Price = price,
                Stock = stock,
                UnitOfMeasure = unitOfMeasure,
                Warehouse = warehouse,
                VatRate = vatRate,
                Utc = utc
            });
        }

        public void UpdateStock(double quantity, string reason, DateTime utc)
        {
            Apply(new SkuStockUpdated
            {
                Id = _state.Id,
                Quantity = quantity,
                Reason = reason,
                Utc = utc
            });
        }

        public void Rename(string title, DateTime utc)
        {
            Apply(new SkuRenamed
            {
                Id = _state.Id,
                NewTitle = title,
                Utc = utc
            });
        }

        public void ChangeBarcode(string barcode, DateTime utc)
        {
            Apply(new SkuBarcodeChanged
            {
                Id = _state.Id,
                NewBarcode = barcode,
                Utc = utc
            });
        }

        public void ChangeItemNo(string itemNo, DateTime utc)
        {
            Apply(new SkuItemNoChanged
            {
                Id = _state.Id,
                NewItemNo = itemNo,
                Utc = utc
            });
        }

        public void ChangePrice(CurrencyAmount price, DateTime utc)
        {
            Apply(new SkuPriceChanged
            {
                Id = _state.Id,
                NewPrice = price,
                Utc = utc
            });
        }

        public void ChangeUnitOfMeasure(string unitOfMeasure, DateTime utc)
        {
            Apply(new SkuUnitOfMeasureChanged
            {
                Id = _state.Id,
                NewUnitOfMeasure = unitOfMeasure,
                Utc = utc
            });
        }

        public void ChangeVatRate(VatRate vat, DateTime utc)
        {
            Apply(new SkuVatRateChanged
            {
                Id = _state.Id,
                NewVatRate = vat,
                Utc = utc
            });
        }

        public void MoveToWarehouse(WarehouseId warehouse, string reason, DateTime utc)
        {
            Apply(new SkuMovedToWarehouse
            {
                Id = _state.Id,
                DestinationWarehouse = warehouse,
                Reason = reason,
                Utc = utc
            });
        }

        public IList<IEvent> Changes { get { return _changes; } }
    }
}
