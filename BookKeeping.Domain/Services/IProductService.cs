using System;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.Services
{
    public interface IProductService
    {
        long GetStoreId(string productIdentifier);

        string GetPropertyValue(string productIdentifier, string propertyAlias);

        string GetSku(string productIdentifier);

        long? GetVatGroupId(string productIdentifier);

        long? GetLanguageId(string productIdentifier);

        OriginalUnitPriceCollection GetOriginalUnitPrices(string productIdentifier);

        CustomPropertyCollection GetProperties(string productIdentifier);

        ProductSnapshot GetSnapshot(string productIdentifier);

        Decimal? GetStock(long storeId, string sku);

        void SetStock(long storeId, string sku, Decimal? value);

        bool HasAccess(long storeId, string productIdentifier);
    }
}