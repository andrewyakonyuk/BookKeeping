using System.Collections.Generic;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.Services
{
    public interface ICurrencyService
    {
        IEnumerable<Currency> GetAll(long storeId);

        IEnumerable<Currency> GetAllAllowedIn(long storeId, long countryId);

        Currency Get(long storeId, long currencyId);
    }
}