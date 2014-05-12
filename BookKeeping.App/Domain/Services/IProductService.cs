using System;

namespace BookKeeping.App.Domain.Services
{
    public interface IProductService
    {
        Decimal? GetStock(string itemNo);

        void SetStock(string itemNo, Decimal? stock);
    }
}
