using BookKeeping.Core.Domain;
using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts.Product.Commands
{
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class MakeProductNonOrderable : ICommand<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }

        [DataMember(Order = 2)]
        public string Reason { get; set; }

        public override string ToString()
        {
            return string.Format("Make {0} non-orderable, because {1}", Id, Reason);
        }
    }
}
