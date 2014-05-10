using BookKeeping.Core.Domain;
using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts.Product.Events
{
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class ProductUnitOfMeasureChanged : IEvent<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
        [DataMember(Order = 2)]
        public string NewUnitOfMeasure { get; set; }
        [DataMember(Order = 3)]
        public DateTime Utc { get; set; }

        public override string ToString()
        {
            return string.Format("{0} changed UOM to '{1}'", Id, NewUnitOfMeasure);
        }
    }
}
