using BookKeeping.Core.Domain;
using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts.Store.Events
{
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class StoreClosed : IEvent<StoreId>
    {
        [DataMember(Order = 1)]
        public StoreId Id { get; set; }

        [DataMember(Order = 2)]
        public string Reason { get; set; }

        [DataMember(Order = 3)]
        public DateTime Utc { get; set; }
    }
}
