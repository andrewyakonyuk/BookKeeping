using System.Collections.Generic;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.Repositories
{
    public interface ICountryRepository
    {
        IEnumerable<Country> GetAll(long storeId);

        Country Get(long storeId, long countryId);

        void Save(Country country);

        int GetHighestSortValue(long storeId);
    }
}