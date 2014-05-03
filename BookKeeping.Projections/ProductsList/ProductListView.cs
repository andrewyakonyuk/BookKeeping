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
        public string Title { get; set; }

        [DataMember(Order = 3)]
        public string Barcode { get; set; }

        [DataMember(Order = 4)]
        public string ItemNo { get; set; }

        [DataMember(Order = 5)]
        public CurrencyAmount Price { get; set; }

        [DataMember(Order = 6)]
        public double Stock { get; set; }

        [DataMember(Order = 7)]
        public string UOM { get; set; }

        [DataMember(Order = 8)]
        public double VAT { get; set; }
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
