using BookKeeping.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Domain.ProductAggregate
{
    [Serializable]
    public sealed class CreateProduct : ICommand<ProductId>
    {
        public ProductId Id { get; set; }
        public string Title { get; set; }
        public string ItemNo { get; set; }
        public CurrencyAmount Price { get; set; }
        public double Stock { get; set; }

        public override string ToString()
        {
            return string.Format("Create {0} named '{1}' with item no. {2}, price {3}, stock {4}", Id, Title, ItemNo, Price, Stock);
        }
    }

    [Serializable]
    public sealed class ProductCreated : IEvent<ProductId>
    {
        public ProductId Id { get; set; }
        public string Title { get; set; }
        public string ItemNo { get; set; }
        public CurrencyAmount Price { get; set; }
        public double Stock { get; set; }
        public DateTime Created { get; set; }

        public override string ToString()
        {
            return string.Format("{0} created named '{1}' with item no. {2}, price {3}, stock {4}", Id, Title, ItemNo, Price, Stock);
        }
    }

    [Serializable]
    public sealed class UpdateProductStock : ICommand<ProductId>
    {
        public ProductId Id { get; set; }
        public double Quantity { get; set; }
        public string Reason { get; set; }

        public override string ToString()
        {
            return string.Format("Update {0} stock with quantity {1} becouse '{2}'", Id, Quantity, Reason);
        }
    }

    [Serializable]
    public sealed class ProductStockUpdated : IEvent<ProductId>
    {
        public ProductId Id { get; set; }
        public double Quantity { get; set; }
        public string Reason { get; set; }
        public DateTime Updated { get; set; }

        public override string ToString()
        {
            return string.Format("{0} was updated with quantity {1} becouse '{2}'", Id, Quantity, Reason);
        }
    }

    [Serializable]
    public sealed class RenameProduct : ICommand<ProductId>
    {
        public ProductId Id { get; set; }
        public string NewTitle { get; set; }

        public override string ToString()
        {
            return string.Format("Rename {0} to '{1}'", Id, NewTitle);
        }
    }

    [Serializable]
    public sealed class ProductRenamed : IEvent<ProductId>
    {
        public ProductId Id { get; set; }
        public string NewTitle { get; set; }
        public DateTime Renamed { get; set; }

        public override string ToString()
        {
            return string.Format("{0} renamed to '{1}'", Id, NewTitle);
        }
    }

    [Serializable]
    public sealed class ChangeProductBarcode : ICommand<ProductId>
    {
        public ProductId Id { get; set; }
        public string NewBarcode { get; set; }

        public override string ToString()
        {
            return string.Format("Change {0} barcode to {1}", Id, NewBarcode);
        }
    }

    [Serializable]
    public sealed class ProductBarcodeChanged : IEvent<ProductId>
    {
        public ProductId Id { get; set; }
        public string NewBarcode { get; set; }
        public DateTime Changed { get; set; }

        public override string ToString()
        {
            return string.Format("{0} changed barcode to {1}", Id, NewBarcode);
        }
    }

    [Serializable]
    public sealed class ChangeProductItemNo : ICommand<ProductId>
    {
        public ProductId Id { get; set; }
        public string NewItemNo { get; set; }

        public override string ToString()
        {
            return string.Format("Change {0} item no. to {1}", Id, NewItemNo);
        }
    }

    [Serializable]
    public sealed class ProductItemNoChanged : IEvent<ProductId>
    {
        public ProductId Id { get; set; }
        public string NewItemNo { get; set; }
        public DateTime Changed { get; set; }

        public override string ToString()
        {
            return string.Format("{0} changed item no. to {1}", Id, NewItemNo);
        }
    }

    [Serializable]
    public sealed class ChangeProductPrice : ICommand<ProductId>
    {
        public ProductId Id { get; set; }
        public CurrencyAmount NewPrice { get; set; }

        public override string ToString()
        {
            return string.Format("Change {0} price to {1}", Id, NewPrice);
        }
    }

    public sealed class ProductPriceChanged : IEvent<ProductId>
    {
        public ProductId Id { get; set; }
        public CurrencyAmount NewPrice { get; set; }
        public DateTime Changed { get; set; }

        public override string ToString()
        {
            return string.Format("{0} changed price to {1}", Id, NewPrice);
        }
    }

    [Serializable]
    public sealed class ChangeProductUOM : ICommand<ProductId>
    {
        public ProductId Id { get; set; }
        public string NewUOM { get; set; }

        public override string ToString()
        {
            return string.Format("Change {0} UOM to '{1}'", Id, NewUOM);
        }
    }

    [Serializable]
    public sealed class ProductUOMChanged : IEvent<ProductId>
    {
        public ProductId Id { get; set; }
        public string NewUOM { get; set; }
        public DateTime Changed { get; set; }

        public override string ToString()
        {
            return string.Format("{0} changed UOM to '{1}'", Id, NewUOM);
        }
    }

    [Serializable]
    public sealed class ChangeProductVAT : ICommand<ProductId>
    {
        public ProductId Id { get; set; }
        public double NewVAT { get; set; }

        public override string ToString()
        {
            return string.Format("Change {0} VAT to '{1}'", Id, NewVAT);
        }
    }

    [Serializable]
    public sealed class ProductVATChanged : IEvent<ProductId>
    {
        public ProductId Id { get; set; }
        public double NewVAT { get; set; }
        public DateTime Changed { get; set; }

        public override string ToString()
        {
            return string.Format("{0} changed VAT to '{1}'", Id, NewVAT);
        }
    }
}
