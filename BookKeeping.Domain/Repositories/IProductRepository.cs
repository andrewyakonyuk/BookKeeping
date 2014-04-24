using System;

namespace BookKeeping.Domain.Repositories
{
    public interface IProductRepository
    {
        Decimal? GetStock(long storeId, string sku);

        void SetStock(long storeId, string sku, Decimal? value);
    }
}