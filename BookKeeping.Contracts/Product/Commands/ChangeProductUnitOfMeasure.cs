using BookKeeping.Core.Domain;
using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts.Product.Commands
{
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class ChangeProductUnitOfMeasure : ICommand<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
        [DataMember(Order = 2)]
        public string NewUnitOfMeasure { get; set; }

        public override string ToString()
        {
            return string.Format("Change {0} UOM to '{1}'", Id, NewUnitOfMeasure);
        }
    }
}
