using BookKeeping.Core.Domain;
using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts.Product.Commands
{
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class ChangeProductVatRate : ICommand<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
        [DataMember(Order = 2)]
        public VatRate NewVatRate { get; set; }

        public override string ToString()
        {
            return string.Format("Change {0} Vat rate to '{1}'", Id, NewVatRate);
        }
    }
}
