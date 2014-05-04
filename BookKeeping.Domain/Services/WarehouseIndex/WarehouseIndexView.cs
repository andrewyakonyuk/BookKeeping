using BookKeeping.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BookKeeping.Domain.Services.WarehouseIndex
{
    [DataContract]
    public sealed class WarehouseIndexView
    {
        [DataMember(Order = 1)]
        public WarehouseId Id { get; set; }

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
        public SkuId Id { get; set; }
    }
}
