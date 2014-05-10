using BookKeeping.Core.Domain;
using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts.Store.Commands
{
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class RenameStore : ICommand<StoreId>
    {
        [DataMember(Order = 1)]
        public StoreId Id { get; set; }

        [DataMember(Order = 2)]
        public string NewName { get; set; }
    }
}
