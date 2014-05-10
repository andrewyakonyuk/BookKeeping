using BookKeeping.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BookKeeping.Projections.ProductsList
{
    [DataContract]
    [Serializable]
    public class ProductView
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }

        [DataMember(Order = 2)]
        public WarehouseId Warehouse { get; set; }

        [DataMember(Order = 3)]
        public string Title { get; set; }

        [DataMember(Order = 4)]
        public string Barcode { get; set; }

        [DataMember(Order = 5)]
        public string ItemNo { get; set; }

        [DataMember(Order = 6)]
        public CurrencyAmount Price { get; set; }

        [DataMember(Order = 7)]
        public double Stock { get; set; }

        [DataMember(Order = 8)]
        public string UnitOfMeasure { get; set; }

        [DataMember(Order = 9)]
        public VatRate VatRate { get; set; }
    }

    [DataContract]
    [Serializable]
    public class ProductListView
    {
        private IList<ProductView> _products = new List<ProductView>();

        [DataMember(Order = 1)]
        public IList<ProductView> Products { get { return _products; } }
    }
}
