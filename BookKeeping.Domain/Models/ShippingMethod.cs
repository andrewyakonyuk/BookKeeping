using System.Collections.Generic;
using System.Linq;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.PriceCalculators;
using BookKeeping.Domain.Repositories;
using BookKeeping.Domain.Services;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Models
{
    public class ShippingMethod : ISortable
    {
        public long Id
        {
            get;
            set;
        }

        public long StoreId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Alias
        {
            get;
            set;
        }

        public string ImageIdentifier
        {
            get;
            set;
        }

        public long? VatGroupId
        {
            get;
            set;
        }

        public string Sku
        {
            get;
            set;
        }

        public int Sort
        {
            get;
            set;
        }

        public bool IsDeleted
        {
            get;
            set;
        }

        public ServicePriceCollection OriginalPrices
        {
            get;
            set;
        }

        public IList<long> AllowedInFollowingCountries
        {
            get;
            set;
        }

        public IList<long> AllowedInFollowingCountryRegions
        {
            get;
            set;
        }

        public ShippingMethod()
        {
            this.OriginalPrices = new ServicePriceCollection();
            this.AllowedInFollowingCountries = new List<long>();
            this.AllowedInFollowingCountryRegions = new List<long>();
            this.Sort = -1;
        }

        public ShippingMethod(long storeId, string name)
            : this()
        {
            this.StoreId = storeId;
            this.Name = name;
            this.Alias = name.ToCamelCase();
        }

        public void Save()
        {
            bool flag = this.Id == 0L;
            if (!flag)
            {
                ShippingMethod shippingMethod = DependencyResolver.Current.GetService<IShippingMethodRepository>().Get(this.StoreId, this.Id);
                foreach (var countryId in from i in shippingMethod.AllowedInFollowingCountries
                                          where !this.AllowedInFollowingCountries.Contains(i)
                                          select i)
                {
                    Country country = CountryService.Instance.Get(this.StoreId, countryId);
                    if (country.DefaultPaymentMethodId == this.Id)
                    {
                        country.DefaultPaymentMethodId = null;
                        country.Save();
                    }
                    this.OriginalPrices.RemoveAll((ServicePrice p) => p.CountryId == countryId);
                }
                foreach (long current in
                    from i in shippingMethod.AllowedInFollowingCountryRegions
                    where !this.AllowedInFollowingCountryRegions.Contains(i)
                    select i)
                {
                    CountryRegion countryRegion = CountryRegionService.Instance.Get(this.StoreId, current);
                    if (countryRegion.DefaultPaymentMethodId == this.Id)
                    {
                        countryRegion.DefaultPaymentMethodId = null;
                        countryRegion.Save();
                    }
                    this.OriginalPrices.RemoveAll((ServicePrice p) => p.CountryRegionId == countryRegion.Id);
                }
            }
            IShippingMethodRepository shippingMethodRepository = DependencyResolver.Current.GetService<IShippingMethodRepository>();
            if (this.Sort == -1)
            {
                this.Sort = shippingMethodRepository.GetHighestSortValue(this.StoreId) + 1;
            }
            shippingMethodRepository.Save(this);
            if (flag)
            {
                NotificationCenter.ShippingMethod.OnCreated(this);
            }
        }

        public bool Delete()
        {
            this.IsDeleted = true;
            this.Save();
            foreach (Country current in CountryService.Instance.GetAll(this.StoreId))
            {
                if (current.DefaultShippingMethodId == this.Id)
                {
                    current.DefaultShippingMethodId = null;
                    current.Save();
                }
            }
            foreach (CountryRegion current2 in CountryRegionService.Instance.GetAll(this.StoreId, null))
            {
                if (current2.DefaultShippingMethodId == this.Id)
                {
                    current2.DefaultShippingMethodId = null;
                    current2.Save();
                }
            }
            NotificationCenter.ShippingMethod.OnDeleted(this);
            return true;
        }

        public bool IsAllowedInRegion(long countryId, long? countryRegionId)
        {
            bool result = false;
            if (!this.IsDeleted && this.AllowedInFollowingCountries.Contains(countryId))
            {
                result = (!countryRegionId.HasValue || this.AllowedInFollowingCountryRegions.Contains(countryRegionId.Value));
            }
            return result;
        }

        public decimal GetOriginalPrice(long currencyId, long countryId, long? countryRegionId)
        {
            decimal? price = this.OriginalPrices.GetPrice(currencyId, new long?(countryId), countryRegionId);
            if (price.HasValue)
            {
                return price.GetValueOrDefault();
            }
            decimal? price2 = this.OriginalPrices.GetPrice(currencyId, new long?(countryId), null);
            if (price2.HasValue)
            {
                return price2.GetValueOrDefault();
            }
            decimal? price3 = this.OriginalPrices.GetPrice(currencyId, null, null);
            if (!price3.HasValue)
            {
                return 0m;
            }
            return price3.GetValueOrDefault();
        }

        public IEnumerable<Price> CalculatePrices(Order order)
        {
            return DependencyResolver.Current.GetService<IShippingCalculator>().CalculatePrices(this, order);
        }

        public IEnumerable<Price> CalculatePrices(long countryId, long? countryRegionId, VatRate fallbackVatRate)
        {
            IShippingCalculator shippingCalculator = DependencyResolver.Current.GetService<IShippingCalculator>();
            VatRate vatRate = shippingCalculator.CalculateVatRate(this, countryId, countryRegionId, fallbackVatRate);
            return shippingCalculator.CalculatePrices(this, countryId, countryRegionId, vatRate);
        }
    }
}