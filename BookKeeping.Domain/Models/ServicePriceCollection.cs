using System;
using System.Collections.Generic;
using System.Linq;

namespace BookKeeping.Domain.Models
{
    public class ServicePriceCollection : List<ServicePrice>
    {
        public ServicePrice Get(long currencyId, long? countryId = null, long? countryRegionId = null)
        {
            return Enumerable.SingleOrDefault<ServicePrice>((IEnumerable<ServicePrice>)this, (Func<ServicePrice, bool>)(p =>
            {
                if (p.CurrencyId == currencyId)
                {
                    long? local_0 = p.CountryId;
                    long? local_1 = countryId;
                    if ((local_0.GetValueOrDefault() != local_1.GetValueOrDefault() ? 0 : (local_0.HasValue == local_1.HasValue ? 1 : 0)) != 0)
                    {
                        long? local_2 = p.CountryRegionId;
                        long? local_3 = countryRegionId;
                        if (local_2.GetValueOrDefault() == local_3.GetValueOrDefault())
                            return local_2.HasValue == local_3.HasValue;
                        else
                            return false;
                    }
                }
                return false;
            }));
        }

        public Decimal? GetPrice(long currencyId, long? countryId = null, long? countryRegionId = null)
        {
            Decimal? nullable = new Decimal?();
            ServicePrice servicePrice = this.Get(currencyId, countryId, countryRegionId);
            if (servicePrice != null)
                nullable = new Decimal?(servicePrice.Value);
            return nullable;
        }
    }
}