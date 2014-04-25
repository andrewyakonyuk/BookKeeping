using System.Collections.Generic;
using System.Linq;

namespace BookKeeping.Domain.Models
{
    public class ServicePriceCollection : List<ServicePrice>
    {
        public ServicePrice Get(long currencyId, long? countryId = null, long? countryRegionId = null)
        {
            return this.SingleOrDefault((ServicePrice p) => p.CurrencyId == currencyId && p.CountryId == countryId && p.CountryRegionId == countryRegionId);
        }

        public decimal? GetPrice(long currencyId, long? countryId = null, long? countryRegionId = null)
        {
            decimal? result = null;
            ServicePrice servicePrice = this.Get(currencyId, countryId, countryRegionId);
            if (servicePrice != null)
            {
                result = new decimal?(servicePrice.Value);
            }
            return result;
        }
    }
}