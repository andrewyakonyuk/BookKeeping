using BookKeeping.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BookKeeping.Domain.Contracts
{
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class CreateWarehouse : ICommand<WarehouseId>
    {
        [DataMember(Order = 1)]
        public WarehouseId Id { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class WarehouseCreated : IEvent<WarehouseId>
    {
        [DataMember(Order = 1)]
        public WarehouseId Id { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public DateTime Created { get; set; }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class CloseWarehouse : ICommand<WarehouseId>
    {
        [DataMember(Order = 1)]
        public WarehouseId Id { get; set; }

        [DataMember(Order = 2)]
        public string Reason { get; set; }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class WarehouseClosed : IEvent<WarehouseId>
    {
        [DataMember(Order = 1)]
        public WarehouseId Id { get; set; }

        [DataMember(Order = 2)]
        public string Reason { get; set; }

        [DataMember(Order = 3)]
        public DateTime Closed { get; set; }
    }

    [DataContract(Namespace = "BookKeeping")]
    public sealed class RenameWarehouse : ICommand<WarehouseId>
    {
        [DataMember(Order = 1)]
        public WarehouseId Id { get; set; }

        [DataMember(Order = 2)]
        public string NewName { get; set; }
    }

    [DataContract(Namespace = "BookKeeping")]
    public sealed class WarehouseRenamed : IEvent<WarehouseId>
    {
        [DataMember(Order = 1)]
        public WarehouseId Id { get; set; }

        [DataMember(Order = 2)]
        public string NewName { get; set; }

        [DataMember(Order = 3)]
        public DateTime Renamed { get; set; }
    }
}
