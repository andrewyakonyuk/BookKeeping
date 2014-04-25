using System.Collections.Generic;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Domain.Services;
using BookKeeping.Infrastructure.Dependency;
using BookKeeping.Infrastructure.Domain;
using BookKeeping.Infrastructure.Logging;

namespace BookKeeping.Domain.Models
{
    public class Country : ISortable, IEntity
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

        public long DefaultCurrencyId
        {
            get;
            set;
        }

        public string RegionCode
        {
            get;
            set;
        }

        public long? DefaultShippingMethodId
        {
            get;
            set;
        }

        public long? DefaultPaymentMethodId
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

        public Country()
        {
            this.Sort = -1;
        }

        public Country(long storeId, string name, long defaultCurrencyId)
            : this()
        {
            this.StoreId = storeId;
            this.Name = name;
            this.DefaultCurrencyId = defaultCurrencyId;
        }

        public void Save()
        {
            bool flag = this.Id == 0L;
            ICountryRepository countryRepository = DependencyResolver.Current.GetService<ICountryRepository>();
            if (this.Sort == -1)
            {
                this.Sort = countryRepository.GetHighestSortValue(this.StoreId) + 1;
            }
            countryRepository.Save(this);
            Currency currency = CurrencyService.Instance.Get(this.StoreId, this.DefaultCurrencyId);
            if (!currency.AllowedInFollowingCountries.Contains(this.Id) && !this.IsDeleted)
            {
                currency.AllowedInFollowingCountries.Add(this.Id);
                currency.Save();
            }
            if (this.DefaultShippingMethodId.HasValue && !this.IsDeleted)
            {
                ShippingMethod shippingMethod = ShippingMethodService.Instance.Get(this.StoreId, this.DefaultShippingMethodId.Value);
                if (!shippingMethod.AllowedInFollowingCountries.Contains(this.Id))
                {
                    shippingMethod.AllowedInFollowingCountries.Add(this.Id);
                    shippingMethod.Save();
                }
            }
            if (this.DefaultPaymentMethodId.HasValue && !this.IsDeleted)
            {
                PaymentMethod paymentMethod = PaymentMethodService.Instance.Get(this.StoreId, this.DefaultPaymentMethodId.Value);
                if (!paymentMethod.AllowedInFollowingCountries.Contains(this.Id))
                {
                    paymentMethod.AllowedInFollowingCountries.Add(this.Id);
                    paymentMethod.Save();
                }
            }
            if (flag)
            {
                NotificationCenter.Country.OnCreated(this);
            }
        }

        public bool Delete()
        {
            bool result = false;
            Store store = StoreService.Instance.Get(this.StoreId);
            if (store.DefaultCountryId != this.Id)
            {
                this.IsDeleted = true;
                this.Save();
                result = true;
                foreach (CountryRegion current in CountryRegionService.Instance.GetAll(this.StoreId, new long?(this.Id)))
                {
                    current.Delete();
                }
                IEnumerable<Currency> allAllowedIn = CurrencyService.Instance.GetAllAllowedIn(this.StoreId, this.Id);
                foreach (Currency current2 in allAllowedIn)
                {
                    current2.AllowedInFollowingCountries.Remove(this.Id);
                    current2.Save();
                }
                IEnumerable<ShippingMethod> allAllowedIn2 = ShippingMethodService.Instance.GetAllAllowedIn(this.StoreId, this.Id, null);
                foreach (ShippingMethod current3 in allAllowedIn2)
                {
                    current3.AllowedInFollowingCountries.Remove(this.Id);
                    current3.OriginalPrices.RemoveAll((ServicePrice p) => p.CountryId == this.Id);
                    current3.Save();
                }
                IEnumerable<PaymentMethod> allAllowedIn3 = PaymentMethodService.Instance.GetAllAllowedIn(this.StoreId, this.Id, null);
                foreach (PaymentMethod current4 in allAllowedIn3)
                {
                    current4.AllowedInFollowingCountries.Remove(this.Id);
                    current4.OriginalPrices.RemoveAll((ServicePrice p) => p.CountryId == this.Id);
                    current4.Save();
                }
                IEnumerable<VatGroup> all = VatGroupService.Instance.GetAll(this.StoreId);
                foreach (VatGroup current5 in all)
                {
                    if (current5.CountrySpecificVatRates.ContainsKey(this.Id))
                    {
                        current5.CountrySpecificVatRates.Remove(this.Id);
                        current5.Save();
                    }
                }
                NotificationCenter.Country.OnDeleted(this);
            }
            else
            {
                LoggingService.Instance.Log("Can't delete the country " + this.Name + " because it is the default for the store " + store.Name);
            }
            return result;
        }
    }
}