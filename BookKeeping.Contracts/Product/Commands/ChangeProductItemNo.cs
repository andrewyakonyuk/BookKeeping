using BookKeeping.Core.Domain;
using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts.Product.Commands
{
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class ChangeProductItemNo : ICommand<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
        [DataMember(Order = 2)]
        public string NewItemNo { get; set; }

        public override string ToString()
        {
            return string.Format("Change {0} item no. to {1}", Id, NewItemNo);
        }
    }
}
