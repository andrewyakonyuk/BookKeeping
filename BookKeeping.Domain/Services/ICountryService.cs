using System.Collections.Generic;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.Services
{
    public interface ICountryService
    {
        IEnumerable<Country> GetAll(long storeId);

        Country Get(long storeId, long countryId);
    }
}