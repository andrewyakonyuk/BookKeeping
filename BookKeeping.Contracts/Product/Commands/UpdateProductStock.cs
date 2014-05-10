using BookKeeping.Core.Domain;
using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts.Product.Commands
{
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class UpdateProductStock : ICommand<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
        [DataMember(Order = 2)]
        public double Quantity { get; set; }
        [DataMember(Order = 3)]
        public string Reason { get; set; }

        public override string ToString()
        {
            return string.Format("Update {0} stock with quantity {1} becouse '{2}'", Id, Quantity, Reason);
        }
    }
}
