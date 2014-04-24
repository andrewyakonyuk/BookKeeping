using System;

namespace BookKeeping.Domain.Models
{
    public class ServicePrice
    {
        public long Id { get; set; }

        public long CurrencyId { get; set; }

        public long? CountryId { get; set; }

        public long? CountryRegionId { get; set; }

        public Decimal Value { get; set; }

        public ServicePrice()
        {
        }

        public ServicePrice(long currencyId, Decimal value)
        {
            this.CurrencyId = currencyId;
            this.Value = value;
        }

        public ServicePrice(long currencyId, Decimal value, long countryId, long? countryRegionId = null)
        {
            this.CurrencyId = currencyId;
            this.Value = value;
            this.CountryId = new long?(countryId);
            this.CountryRegionId = countryRegionId;
        }
    }
}