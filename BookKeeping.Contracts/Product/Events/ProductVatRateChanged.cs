using BookKeeping.Core.Domain;
using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts.Product.Events
{
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class ProductVatRateChanged : IEvent<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
        [DataMember(Order = 2)]
        public VatRate NewVatRate { get; set; }
        [DataMember(Order = 3)]
        public DateTime Utc { get; set; }

        public override string ToString()
        {
            return string.Format("{0} changed Vat rate to '{1}'", Id, NewVatRate);
        }
    }
}
