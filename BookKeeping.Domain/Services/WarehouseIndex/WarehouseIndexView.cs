using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Contracts.Product;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Services.WarehouseIndex
{
    [DataContract]
    public sealed class WarehouseIndexView
    {
        [DataMember(Order = 1)]
        public WarehouseId Id { get; set; }

        [DataMember(Order = 2)]
        public List<SkuIndexView> Skus { get; private set; }

        public WarehouseIndexView()
        {
            Skus = new List<SkuIndexView>();
        }
    }

    [DataContract]
    public sealed class SkuIndexView
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
    }
}
