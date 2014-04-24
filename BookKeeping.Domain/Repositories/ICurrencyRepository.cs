using System.Collections.Generic;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.Repositories
{
    public interface ICurrencyRepository
    {
        IEnumerable<Currency> GetAll(long storeId);

        Currency Get(long storeId, long currencyId);

        void Save(Currency currency);

        int GetHighestSortValue(long storeId);
    }
}