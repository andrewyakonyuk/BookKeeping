using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Contracts.Product;
using BookKeeping.Domain.Contracts.Store;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Services.StoreIndex
{
    [DataContract]
    public sealed class StoreIndexView
    {
        [DataMember(Order = 1)]
        public StoreId Id { get; set; }

        [DataMember(Order = 2)]
        public List<ProductIndexView> Products { get; private set; }

        public StoreIndexView()
        {
            Products = new List<ProductIndexView>();
        }
    }

    [DataContract]
    public sealed class ProductIndexView
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
    }
}
