using System.Collections.Generic;
using System.Linq;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.PriceCalculators;
using BookKeeping.Domain.Repositories;
using BookKeeping.Domain.Services;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Models
{
    public class PaymentMethod : ISortable
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

        public string PaymentProviderAlias
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

        public bool AllowsCapturingOfPayment
        {
            get;
            set;
        }

        public bool AllowsCancellationOfPayment
        {
            get;
            set;
        }

        public bool AllowsRetrievalOfPaymentStatus
        {
            get;
            set;
        }

        public bool AllowsRefundOfPayment
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

        public IList<PaymentMethodSetting> Settings
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

        public PaymentMethod()
        {
            this.Settings = new List<PaymentMethodSetting>();
            this.OriginalPrices = new ServicePriceCollection();
            this.AllowedInFollowingCountries = new List<long>();
            this.AllowedInFollowingCountryRegions = new List<long>();
            this.Sort = -1;
        }

        public PaymentMethod(long storeId, string name)
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
                PaymentMethod paymentMethod = DependencyResolver.Current.GetService<IPaymentMethodRepository>().Get(this.StoreId, this.Id);
                foreach (var countryId in from i in paymentMethod.AllowedInFollowingCountries
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
                foreach (var countryRegionId in from i in paymentMethod.AllowedInFollowingCountryRegions
                                                where !this.AllowedInFollowingCountryRegions.Contains(i)
                                                select i)
                {
                    CountryRegion countryRegion = CountryRegionService.Instance.Get(this.StoreId, countryRegionId);
                    if (countryRegion.DefaultPaymentMethodId == this.Id)
                    {
                        countryRegion.DefaultPaymentMethodId = null;
                        countryRegion.Save();
                    }
                    this.OriginalPrices.RemoveAll((ServicePrice p) => p.CountryRegionId == countryRegionId);
                }
            }
            IPaymentMethodRepository paymentMethodRepository = DependencyResolver.Current.GetService<IPaymentMethodRepository>();
            if (this.Sort == -1)
            {
                this.Sort = paymentMethodRepository.GetHighestSortValue(this.StoreId) + 1;
            }
            paymentMethodRepository.Save(this);
            if (flag)
            {
                NotificationCenter.PaymentMethod.OnCreated(this);
            }
        }

        public bool Delete()
        {
            foreach (Country country in CountryService.Instance.GetAll(this.StoreId))
            {
                if (country.DefaultPaymentMethodId == this.Id)
                {
                    country.DefaultPaymentMethodId = null;
                    country.Save();
                }
            }
            foreach (CountryRegion country in CountryRegionService.Instance.GetAll(this.StoreId, null))
            {
                if (country.DefaultPaymentMethodId == this.Id)
                {
                    country.DefaultPaymentMethodId = null;
                    country.Save();
                }
            }
            this.IsDeleted = true;
            this.Save();
            NotificationCenter.PaymentMethod.OnDeleted(this);
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
            return DependencyResolver.Current.GetService<IPaymentCalculator>().CalculatePrices(this, order);
        }

        public IEnumerable<Price> CalculatePrices(long countryId, long? countryRegionId, VatRate fallbackVatRate)
        {
            IPaymentCalculator paymentCalculator = DependencyResolver.Current.GetService<IPaymentCalculator>();
            VatRate vatRate = paymentCalculator.CalculateVatRate(this, countryId, countryRegionId, fallbackVatRate);
            return paymentCalculator.CalculatePrices(this, countryId, countryRegionId, vatRate);
        }

        public IDictionary<string, string> GetMergedSettings(long? languageId)
        {
            Dictionary<string, string> dictionary = (
                from s in this.Settings
                where !s.LanguageId.HasValue
                select s).ToDictionary((PaymentMethodSetting s) => s.Key, (PaymentMethodSetting s) => s.Value);
            if (languageId.HasValue)
            {
                Dictionary<string, string> dictionary2 = (
                    from s in this.Settings
                    where s.LanguageId == languageId
                    select s).ToDictionary((PaymentMethodSetting kvp) => kvp.Key, (PaymentMethodSetting kvp) => kvp.Value);
                foreach (KeyValuePair<string, string> current in dictionary2)
                {
                    dictionary[current.Key] = current.Value;
                }
            }
            return dictionary;
        }
    }
}