using System.Collections.Generic;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.Repositories
{
    public interface ICountryRegionRepository
    {
        IEnumerable<CountryRegion> GetAll(long storeId, long? countryId = null);

        CountryRegion Get(long storeId, long countryRegionId);

        void Save(CountryRegion countryRegion);

        int GetHighestSortValue(long storeId, long countryId);
    }
}