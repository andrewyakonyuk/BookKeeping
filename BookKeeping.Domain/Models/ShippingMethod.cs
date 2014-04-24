using System;
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
        public long Id { get; set; }

        public long StoreId { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public string ImageIdentifier { get; set; }

        public long? VatGroupId { get; set; }

        public string Sku { get; set; }

        public int Sort { get; set; }

        public bool IsDeleted { get; set; }

        public ServicePriceCollection OriginalPrices { get; set; }

        public IList<long> AllowedInFollowingCountries { get; set; }

        public IList<long> AllowedInFollowingCountryRegions { get; set; }

        public ShippingMethod()
        {
            this.OriginalPrices = new ServicePriceCollection();
            this.AllowedInFollowingCountries = (IList<long>)new List<long>();
            this.AllowedInFollowingCountryRegions = (IList<long>)new List<long>();
            this.Sort = -1;
        }

        public ShippingMethod(long storeId, string name)
            : this()
        {
            this.StoreId = storeId;
            this.Name = name;
            this.Alias = name; //TODO:StringExtensions.ToCamelCase(name);
        }

        public void Save()
        {
            bool flag = this.Id == 0L;
            if (!flag)
            {
                ShippingMethod shippingMethod = DependencyResolver.Current.GetService<IShippingMethodRepository>().Get(this.StoreId, this.Id);
                foreach (long num in Enumerable.Where<long>((IEnumerable<long>)shippingMethod.AllowedInFollowingCountries, (Func<long, bool>)(i => !this.AllowedInFollowingCountries.Contains(i))))
                {
                    long countryId = num;
                    Country country = CountryService.Instance.Get(this.StoreId, countryId);
                    long? defaultPaymentMethodId = country.DefaultPaymentMethodId;
                    long id = this.Id;
                    if ((defaultPaymentMethodId.GetValueOrDefault() != id ? 0 : (defaultPaymentMethodId.HasValue ? 1 : 0)) != 0)
                    {
                        country.DefaultPaymentMethodId = new long?();
                        country.Save();
                    }
                    this.OriginalPrices.RemoveAll((Predicate<ServicePrice>)(p =>
                    {
                        long? local_0 = p.CountryId;
                        long local_1 = countryId;
                        if (local_0.GetValueOrDefault() == local_1)
                            return local_0.HasValue;
                        else
                            return false;
                    }));
                }
                foreach (long countryRegionId in Enumerable.Where<long>((IEnumerable<long>)shippingMethod.AllowedInFollowingCountryRegions, (Func<long, bool>)(i => !this.AllowedInFollowingCountryRegions.Contains(i))))
                {
                    CountryRegion countryRegion = CountryRegionService.Instance.Get(this.StoreId, countryRegionId);
                    long? defaultPaymentMethodId = countryRegion.DefaultPaymentMethodId;
                    long id = this.Id;
                    if ((defaultPaymentMethodId.GetValueOrDefault() != id ? 0 : (defaultPaymentMethodId.HasValue ? 1 : 0)) != 0)
                    {
                        countryRegion.DefaultPaymentMethodId = new long?();
                        countryRegion.Save();
                    }
                    this.OriginalPrices.RemoveAll((Predicate<ServicePrice>)(p =>
                    {
                        long? local_0 = p.CountryRegionId;
                        long local_1 = countryRegion.Id;
                        if (local_0.GetValueOrDefault() == local_1)
                            return local_0.HasValue;
                        else
                            return false;
                    }));
                }
            }
            IShippingMethodRepository methodRepository = DependencyResolver.Current.GetService<IShippingMethodRepository>();
            if (this.Sort == -1)
                this.Sort = methodRepository.GetHighestSortValue(this.StoreId) + 1;
            methodRepository.Save(this);
            if (!flag)
                return;
            NotificationCenter.ShippingMethod.OnCreated(this);
        }

        public bool Delete()
        {
            this.IsDeleted = true;
            this.Save();
            foreach (Country country in CountryService.Instance.GetAll(this.StoreId))
            {
                long? shippingMethodId = country.DefaultShippingMethodId;
                long id = this.Id;
                if ((shippingMethodId.GetValueOrDefault() != id ? 0 : (shippingMethodId.HasValue ? 1 : 0)) != 0)
                {
                    country.DefaultShippingMethodId = new long?();
                    country.Save();
                }
            }
            foreach (CountryRegion countryRegion in CountryRegionService.Instance.GetAll(this.StoreId, new long?()))
            {
                long? shippingMethodId = countryRegion.DefaultShippingMethodId;
                long id = this.Id;
                if ((shippingMethodId.GetValueOrDefault() != id ? 0 : (shippingMethodId.HasValue ? 1 : 0)) != 0)
                {
                    countryRegion.DefaultShippingMethodId = new long?();
                    countryRegion.Save();
                }
            }
            NotificationCenter.ShippingMethod.OnDeleted(this);
            return true;
        }

        public bool IsAllowedInRegion(long countryId, long? countryRegionId)
        {
            bool flag = false;
            if (!this.IsDeleted && this.AllowedInFollowingCountries.Contains(countryId))
                flag = !countryRegionId.HasValue || this.AllowedInFollowingCountryRegions.Contains(countryRegionId.Value);
            return flag;
        }

        public Decimal GetOriginalPrice(long currencyId, long countryId, long? countryRegionId)
        {
            return this.OriginalPrices.GetPrice(currencyId, new long?(countryId), countryRegionId) ?? this.OriginalPrices.GetPrice(currencyId, new long?(countryId), new long?()) ?? this.OriginalPrices.GetPrice(currencyId, new long?(), new long?()) ?? new Decimal(0);
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