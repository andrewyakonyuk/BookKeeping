using BookKeeping.Core;
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
    public sealed class CreateProduct : ICommand<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
        [DataMember(Order = 2)]
        public string Title { get; set; }
        [DataMember(Order = 3)]
        public string ItemNo { get; set; }
        [DataMember(Order = 4)]
        public CurrencyAmount Price { get; set; }
        [DataMember(Order = 5)]
        public double Stock { get; set; }

        public override string ToString()
        {
            return string.Format("Create {0} named '{1}' with item no. {2}, price {3}, stock {4}", Id, Title, ItemNo, Price, Stock);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class ProductCreated : IEvent<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
        [DataMember(Order = 2)]
        public string Title { get; set; }
        [DataMember(Order = 3)]
        public string ItemNo { get; set; }
        [DataMember(Order = 4)]
        public CurrencyAmount Price { get; set; }
        [DataMember(Order = 5)]
        public double Stock { get; set; }
        [DataMember(Order = 6)]
        public DateTime Created { get; set; }

        public override string ToString()
        {
            return string.Format("{0} created named '{1}' with item no. {2}, price {3}, stock {4}", Id, Title, ItemNo, Price, Stock);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class UpdateProductStock : ICommand<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
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
    public sealed class ProductStockUpdated : IEvent<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
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
    public sealed class RenameProduct : ICommand<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
        [DataMember(Order = 2)]
        public string NewTitle { get; set; }

        public override string ToString()
        {
            return string.Format("Rename {0} to '{1}'", Id, NewTitle);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class ProductRenamed : IEvent<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
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
    public sealed class ChangeProductBarcode : ICommand<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
        [DataMember(Order = 2)]
        public string NewBarcode { get; set; }

        public override string ToString()
        {
            return string.Format("Change {0} barcode to {1}", Id, NewBarcode);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class ProductBarcodeChanged : IEvent<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
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
    public sealed class ChangeProductItemNo : ICommand<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
        [DataMember(Order = 2)]
        public string NewItemNo { get; set; }

        public override string ToString()
        {
            return string.Format("Change {0} item no. to {1}", Id, NewItemNo);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class ProductItemNoChanged : IEvent<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
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
    public sealed class ChangeProductPrice : ICommand<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
        [DataMember(Order = 2)]
        public CurrencyAmount NewPrice { get; set; }

        public override string ToString()
        {
            return string.Format("Change {0} price to {1}", Id, NewPrice);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class ProductPriceChanged : IEvent<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
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
    public sealed class ChangeProductUOM : ICommand<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
        [DataMember(Order = 2)]
        public string NewUOM { get; set; }

        public override string ToString()
        {
            return string.Format("Change {0} UOM to '{1}'", Id, NewUOM);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class ProductUOMChanged : IEvent<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
        [DataMember(Order = 2)]
        public string NewUOM { get; set; }
        [DataMember(Order = 3)]
        public DateTime Changed { get; set; }

        public override string ToString()
        {
            return string.Format("{0} changed UOM to '{1}'", Id, NewUOM);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class ChangeProductVAT : ICommand<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
        [DataMember(Order = 2)]
        public double NewVAT { get; set; }

        public override string ToString()
        {
            return string.Format("Change {0} VAT to '{1}'", Id, NewVAT);
        }
    }

    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class ProductVATChanged : IEvent<ProductId>
    {
        [DataMember(Order = 1)]
        public ProductId Id { get; set; }
        [DataMember(Order = 2)]
        public double NewVAT { get; set; }
        [DataMember(Order = 3)]
        public DateTime Changed { get; set; }

        public override string ToString()
        {
            return string.Format("{0} changed VAT to '{1}'", Id, NewVAT);
        }
    }
}
