using System.Collections.Generic;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.Services
{
    public interface ICountryRegionService
    {
        IEnumerable<CountryRegion> GetAll(long storeId, long? countryId = null);

        CountryRegion Get(long storeId, long countryRegionId);
    }
}