using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.InformationExtractors
{
    public interface IProductInformationExtractor
    {
        string GetPropertyValue(string productIdentifier, string propertyAlias);

        long GetStoreId(string productIdentifier);

        string GetSku(string productIdentifier);

        string GetName(string productIdentifier);

        long? GetVatGroupId(string productIdentifier);

        long? GetLanguageId(string productIdentifier);

        OriginalUnitPriceCollection GetOriginalUnitPrices(string productIdentifier);

        CustomPropertyCollection GetProperties(string productIdentifier);

        ProductSnapshot GetSnapshot(string productIdentifier);

        bool HasAccess(long storeId, string productIdentifier);
    }
}