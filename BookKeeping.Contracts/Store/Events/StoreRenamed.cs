﻿using BookKeeping.Core.Domain;
using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts.Store.Events
{
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class StoreRenamed : IEvent<StoreId>
    {
        [DataMember(Order = 1)]
        public StoreId Id { get; set; }

        [DataMember(Order = 2)]
        public string NewName { get; set; }

        [DataMember(Order = 3)]
        public DateTime Utc { get; set; }
    }
}
