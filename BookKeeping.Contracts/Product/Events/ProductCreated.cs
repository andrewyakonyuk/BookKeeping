using BookKeeping.Core.Domain;
using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts.Product.Events
{
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class ProductCreated : IEvent<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
        [DataMember(Order = 2)]
        public WarehouseId Warehouse { get; set; }
        [DataMember(Order = 3)]
        public string Title { get; set; }
        [DataMember(Order = 4)]
        public string ItemNo { get; set; }
        [DataMember(Order = 5)]
        public CurrencyAmount Price { get; set; }
        [DataMember(Order = 6)]
        public double Stock { get; set; }
        [DataMember(Order = 7)]
        public string UnitOfMeasure { get; set; }
        [DataMember(Order = 8)]
        public VatRate VatRate { get; set; }
        [DataMember(Order = 9)]
        public DateTime Utc { get; set; }

        public override string ToString()
        {
            return string.Format("{0} created named '{1}' in {2}  with item no. '{3}', price '{4}',"
                + " stock '{5}', unit of measure '{6}', vat rate '{7}'", Id, Warehouse, Title, ItemNo,
                Price, Stock, UnitOfMeasure, VatRate);
        }
    }
}
