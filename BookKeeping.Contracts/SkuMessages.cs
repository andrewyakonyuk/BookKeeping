using BookKeeping.Core.Domain;
using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts
{
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class CreateSku : ICommand<SkuId>
    {
        [DataMember(Order = 1)]
        public SkuId Id { get; set; }
        [DataMember(Order = 2)]
        public WarehouseId Warehouse { get; set; }
        [DataMember(Order = 3)]
        public string Title { get; set; }
        [DataMember(Order = 4)]
        public string ItemNo { get; set; }
        [DataMember(Order = 5)]
        public CurrencyAmount Price { get; set; }
        [DataMember(Order = 6)]
        public double Stock { get; set; }
        [DataMember(Order = 7)]
        public string UnitOfMeasure { get; set; }
        [DataMember(Order = 8)]
        public VatRate VatRate { get; set; }

        public override string ToString()
        {
            return string.Format("Create {0} named '{1}' in warehouse '{2}'  with item no. '{3}', price '{4}',"
                + " stock '{5}', unit of measure '{6}', vat rate '{7}'", Id, Warehouse, Title, ItemNo, Price,
                Stock, UnitOfMeasure, VatRate);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class SkuCreated : IEvent<SkuId>
    {
        [DataMember(Order = 1)]
        public SkuId Id { get; set; }
        [DataMember(Order = 2)]
        public WarehouseId Warehouse { get; set; }
        [DataMember(Order = 3)]
        public string Title { get; set; }
        [DataMember(Order = 4)]
        public string ItemNo { get; set; }
        [DataMember(Order = 5)]
        public CurrencyAmount Price { get; set; }
        [DataMember(Order = 6)]
        public double Stock { get; set; }
        [DataMember(Order = 7)]
        public string UnitOfMeasure { get; set; }
        [DataMember(Order = 8)]
        public VatRate VatRate { get; set; }
        [DataMember(Order = 9)]
        public DateTime Created { get; set; }

        public override string ToString()
        {
            return string.Format("{0} created named '{1}' in {2}  with item no. '{3}', price '{4}',"
                + " stock '{5}', unit of measure '{6}', vat rate '{7}'", Id, Warehouse, Title, ItemNo,
                Price, Stock, UnitOfMeasure, VatRate);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class UpdateSkuStock : ICommand<SkuId>
    {
        [DataMember(Order = 1)]
        public SkuId Id { get; set; }
        [DataMember(Order = 2)]
        public double Quantity { get; set; }
        [DataMember(Order = 3)]
        public string Reason { get; set; }

        public override string ToString()
        {
            return string.Format("Update {0} stock with quantity {1} becouse '{2}'", Id, Quantity, Reason);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class SkuStockUpdated : IEvent<SkuId>
    {
        [DataMember(Order = 1)]
        public SkuId Id { get; set; }
        [DataMember(Order = 2)]
        public double Quantity { get; set; }
        [DataMember(Order = 3)]
        public string Reason { get; set; }
        [DataMember(Order = 4)]
        public DateTime Updated { get; set; }

        public override string ToString()
        {
            return string.Format("{0} was updated with quantity {1} becouse '{2}'", Id, Quantity, Reason);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class RenameSku : ICommand<SkuId>
    {
        [DataMember(Order = 1)]
        public SkuId Id { get; set; }
        [DataMember(Order = 2)]
        public string NewTitle { get; set; }

        public override string ToString()
        {
            return string.Format("Rename {0} to '{1}'", Id, NewTitle);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class SkuRenamed : IEvent<SkuId>
    {
        [DataMember(Order = 1)]
        public SkuId Id { get; set; }
        [DataMember(Order = 2)]
        public string NewTitle { get; set; }
        [DataMember(Order = 3)]
        public DateTime Renamed { get; set; }

        public override string ToString()
        {
            return string.Format("{0} renamed to '{1}'", Id, NewTitle);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class ChangeSkuBarcode : ICommand<SkuId>
    {
        [DataMember(Order = 1)]
        public SkuId Id { get; set; }
        [DataMember(Order = 2)]
        public string NewBarcode { get; set; }

        public override string ToString()
        {
            return string.Format("Change {0} barcode to {1}", Id, NewBarcode);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class SkuBarcodeChanged : IEvent<SkuId>
    {
        [DataMember(Order = 1)]
        public SkuId Id { get; set; }
        [DataMember(Order = 2)]
        public string NewBarcode { get; set; }
        [DataMember(Order = 2)]
        public DateTime Changed { get; set; }

        public override string ToString()
        {
            return string.Format("{0} changed barcode to {1}", Id, NewBarcode);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class ChangeSkuItemNo : ICommand<SkuId>
    {
        [DataMember(Order = 1)]
        public SkuId Id { get; set; }
        [DataMember(Order = 2)]
        public string NewItemNo { get; set; }

        public override string ToString()
        {
            return string.Format("Change {0} item no. to {1}", Id, NewItemNo);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class SkuItemNoChanged : IEvent<SkuId>
    {
        [DataMember(Order = 1)]
        public SkuId Id { get; set; }
        [DataMember(Order = 2)]
        public string NewItemNo { get; set; }
        [DataMember(Order = 3)]
        public DateTime Changed { get; set; }

        public override string ToString()
        {
            return string.Format("{0} changed item no. to {1}", Id, NewItemNo);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class ChangeSkuPrice : ICommand<SkuId>
    {
        [DataMember(Order = 1)]
        public SkuId Id { get; set; }
        [DataMember(Order = 2)]
        public CurrencyAmount NewPrice { get; set; }

        public override string ToString()
        {
            return string.Format("Change {0} price to {1}", Id, NewPrice);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class SkuPriceChanged : IEvent<SkuId>
    {
        [DataMember(Order = 1)]
        public SkuId Id { get; set; }
        [DataMember(Order = 2)]
        public CurrencyAmount NewPrice { get; set; }
        [DataMember(Order = 3)]
        public DateTime Changed { get; set; }

        public override string ToString()
        {
            return string.Format("{0} changed price to {1}", Id, NewPrice);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class ChangeSkuUnitOfMeasure : ICommand<SkuId>
    {
        [DataMember(Order = 1)]
        public SkuId Id { get; set; }
        [DataMember(Order = 2)]
        public string NewUnitOfMeasure { get; set; }

        public override string ToString()
        {
            return string.Format("Change {0} UOM to '{1}'", Id, NewUnitOfMeasure);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class SkuUnitOfMeasureChanged : IEvent<SkuId>
    {
        [DataMember(Order = 1)]
        public SkuId Id { get; set; }
        [DataMember(Order = 2)]
        public string NewUnitOfMeasure { get; set; }
        [DataMember(Order = 3)]
        public DateTime Changed { get; set; }

        public override string ToString()
        {
            return string.Format("{0} changed UOM to '{1}'", Id, NewUnitOfMeasure);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class ChangeSkuVatRate : ICommand<SkuId>
    {
        [DataMember(Order = 1)]
        public SkuId Id { get; set; }
        [DataMember(Order = 2)]
        public VatRate NewVatRate { get; set; }

        public override string ToString()
        {
            return string.Format("Change {0} Vat rate to '{1}'", Id, NewVatRate);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class SkuVatRateChanged : IEvent<SkuId>
    {
        [DataMember(Order = 1)]
        public SkuId Id { get; set; }
        [DataMember(Order = 2)]
        public VatRate NewVatRate { get; set; }
        [DataMember(Order = 3)]
        public DateTime Changed { get; set; }

        public override string ToString()
        {
            return string.Format("{0} changed Vat rate to '{1}'", Id, NewVatRate);
        }
    }
}
