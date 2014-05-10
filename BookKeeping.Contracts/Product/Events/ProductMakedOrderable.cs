using BookKeeping.Core.Domain;
using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts.Product.Events
{
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class ProductMakedOrderable : IEvent<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }

        [DataMember(Order = 2)]
        public DateTime Utc { get; set; }

        public override string ToString()
        {
            return string.Format("{0} maked orderable", Id);
        }
    }
}
