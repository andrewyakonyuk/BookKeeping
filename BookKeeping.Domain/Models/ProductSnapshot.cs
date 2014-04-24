namespace BookKeeping.Domain.Models
{
    public class ProductSnapshot
    {
        public long StoreId { get; set; }

        public string ProductIdentifier { get; set; }

        public string Sku { get; set; }

        public string Name { get; set; }

        public CustomPropertyCollection Properties { get; set; }

        public OriginalUnitPriceCollection OriginalUnitPrices { get; set; }

        public long? VatGroupId { get; set; }

        public long? LanguageId { get; set; }

        public ProductSnapshot(long storeId, string productIdentifier)
        {
            this.StoreId = storeId;
            this.ProductIdentifier = productIdentifier;
            this.Properties = new CustomPropertyCollection();
            this.OriginalUnitPrices = new OriginalUnitPriceCollection();
        }
    }
}