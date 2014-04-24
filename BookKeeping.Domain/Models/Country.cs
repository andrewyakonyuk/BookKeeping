using System;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Domain.Services;
using BookKeeping.Infrastructure.Dependency;
using BookKeeping.Infrastructure.Logging;

namespace BookKeeping.Domain.Models
{
    public class Country : ISortable
    {
        public long Id { get; set; }

        public long StoreId { get; set; }

        public string Name { get; set; }

        public long DefaultCurrencyId { get; set; }

        public string RegionCode { get; set; }

        public long? DefaultShippingMethodId { get; set; }

        public long? DefaultPaymentMethodId { get; set; }

        public int Sort { get; set; }

        public bool IsDeleted { get; set; }

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
                this.Sort = countryRepository.GetHighestSortValue(this.StoreId) + 1;
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
            if (!flag)
                return;
            NotificationCenter.Country.OnCreated(this);
        }

        public bool Delete()
        {
            bool flag = false;
            Store store = StoreService.Instance.Get(this.StoreId);
            if (store.DefaultCountryId != this.Id)
            {
                this.IsDeleted = true;
                this.Save();
                flag = true;
                foreach (CountryRegion countryRegion in CountryRegionService.Instance.GetAll(this.StoreId, new long?(this.Id)))
                    countryRegion.Delete();
                foreach (Currency currency in CurrencyService.Instance.GetAllAllowedIn(this.StoreId, this.Id))
                {
                    currency.AllowedInFollowingCountries.Remove(this.Id);
                    currency.Save();
                }
                foreach (ShippingMethod shippingMethod in ShippingMethodService.Instance.GetAllAllowedIn(this.StoreId, this.Id, new long?()))
                {
                    shippingMethod.AllowedInFollowingCountries.Remove(this.Id);
                    shippingMethod.OriginalPrices.RemoveAll((Predicate<ServicePrice>)(p =>
                    {
                        long? local_0 = p.CountryId;
                        long local_1 = this.Id;
                        if (local_0.GetValueOrDefault() == local_1)
                            return local_0.HasValue;
                        else
                            return false;
                    }));
                    shippingMethod.Save();
                }
                foreach (PaymentMethod paymentMethod in PaymentMethodService.Instance.GetAllAllowedIn(this.StoreId, this.Id, new long?()))
                {
                    paymentMethod.AllowedInFollowingCountries.Remove(this.Id);
                    paymentMethod.OriginalPrices.RemoveAll((Predicate<ServicePrice>)(p =>
                    {
                        long? local_0 = p.CountryId;
                        long local_1 = this.Id;
                        if (local_0.GetValueOrDefault() == local_1)
                            return local_0.HasValue;
                        else
                            return false;
                    }));
                    paymentMethod.Save();
                }
                foreach (VatGroup vatGroup in VatGroupService.Instance.GetAll(this.StoreId))
                {
                    if (vatGroup.CountrySpecificVatRates.ContainsKey(this.Id))
                    {
                        vatGroup.CountrySpecificVatRates.Remove(this.Id);
                        vatGroup.Save();
                    }
                }
                NotificationCenter.Country.OnDeleted(this);
            }
            else
                LoggingService.Instance.Log("Can't delete the country " + this.Name + " because it is the default for the store " + store.Name);
            return flag;
        }
    }
}