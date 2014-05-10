using BookKeeping.Core.Domain;
using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts.Product.Events
{
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class ProductStockUpdated : IEvent<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
        [DataMember(Order = 2)]
        public double Quantity { get; set; }
        [DataMember(Order = 3)]
        public string Reason { get; set; }
        [DataMember(Order = 4)]
        public DateTime Utc { get; set; }

        public override string ToString()
        {
            return string.Format("{0} was updated with quantity {1} becouse '{2}'", Id, Quantity, Reason);
        }
    }
}
