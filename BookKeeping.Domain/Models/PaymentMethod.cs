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
    public class PaymentMethod : ISortable
    {
        public long Id { get; set; }

        public long StoreId { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public string ImageIdentifier { get; set; }

        public string PaymentProviderAlias { get; set; }

        public long? VatGroupId { get; set; }

        public string Sku { get; set; }

        public bool AllowsCapturingOfPayment { get; set; }

        public bool AllowsCancellationOfPayment { get; set; }

        public bool AllowsRetrievalOfPaymentStatus { get; set; }

        public bool AllowsRefundOfPayment { get; set; }

        public int Sort { get; set; }

        public bool IsDeleted { get; set; }

        public IList<PaymentMethodSetting> Settings { get; set; }

        public ServicePriceCollection OriginalPrices { get; set; }

        public IList<long> AllowedInFollowingCountries { get; set; }

        public IList<long> AllowedInFollowingCountryRegions { get; set; }

        public PaymentMethod()
        {
            this.Settings = (IList<PaymentMethodSetting>)new List<PaymentMethodSetting>();
            this.OriginalPrices = new ServicePriceCollection();
            this.AllowedInFollowingCountries = (IList<long>)new List<long>();
            this.AllowedInFollowingCountryRegions = (IList<long>)new List<long>();
            this.Sort = -1;
        }

        public PaymentMethod(long storeId, string name)
            : this()
        {
            this.StoreId = storeId;
            this.Name = name;
            this.Alias = name;//TODO:StringExtensions.ToCamelCase(name);
        }

        public void Save()
        {
            bool flag = this.Id == 0L;
            if (!flag)
            {
                PaymentMethod paymentMethod = DependencyResolver.Current.GetService<IPaymentMethodRepository>().Get(this.StoreId, this.Id);
                foreach (long num in Enumerable.Where<long>((IEnumerable<long>)paymentMethod.AllowedInFollowingCountries, (Func<long, bool>)(i => !this.AllowedInFollowingCountries.Contains(i))))
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
                foreach (long num in Enumerable.Where<long>((IEnumerable<long>)paymentMethod.AllowedInFollowingCountryRegions, (Func<long, bool>)(i => !this.AllowedInFollowingCountryRegions.Contains(i))))
                {
                    long countryRegionId = num;
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
                        long local_1 = countryRegionId;
                        if (local_0.GetValueOrDefault() == local_1)
                            return local_0.HasValue;
                        else
                            return false;
                    }));
                }
            }
            IPaymentMethodRepository methodRepository = DependencyResolver.Current.GetService<IPaymentMethodRepository>();
            if (this.Sort == -1)
                this.Sort = methodRepository.GetHighestSortValue(this.StoreId) + 1;
            methodRepository.Save(this);
            if (!flag)
                return;
            NotificationCenter.PaymentMethod.OnCreated(this);
        }

        public bool Delete()
        {
            foreach (Country country in CountryService.Instance.GetAll(this.StoreId))
            {
                long? defaultPaymentMethodId = country.DefaultPaymentMethodId;
                long id = this.Id;
                if ((defaultPaymentMethodId.GetValueOrDefault() != id ? 0 : (defaultPaymentMethodId.HasValue ? 1 : 0)) != 0)
                {
                    country.DefaultPaymentMethodId = new long?();
                    country.Save();
                }
            }
            foreach (CountryRegion countryRegion in CountryRegionService.Instance.GetAll(this.StoreId, new long?()))
            {
                long? defaultPaymentMethodId = countryRegion.DefaultPaymentMethodId;
                long id = this.Id;
                if ((defaultPaymentMethodId.GetValueOrDefault() != id ? 0 : (defaultPaymentMethodId.HasValue ? 1 : 0)) != 0)
                {
                    countryRegion.DefaultPaymentMethodId = new long?();
                    countryRegion.Save();
                }
            }
            this.IsDeleted = true;
            this.Save();
            NotificationCenter.PaymentMethod.OnDeleted(this);
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
            Dictionary<string, string> dictionary = Enumerable.ToDictionary<PaymentMethodSetting, string, string>(Enumerable.Where<PaymentMethodSetting>((IEnumerable<PaymentMethodSetting>)this.Settings, (Func<PaymentMethodSetting, bool>)(s => !s.LanguageId.HasValue)), (Func<PaymentMethodSetting, string>)(s => s.Key), (Func<PaymentMethodSetting, string>)(s => s.Value));
            if (languageId.HasValue)
            {
                foreach (KeyValuePair<string, string> keyValuePair in Enumerable.ToDictionary<PaymentMethodSetting, string, string>(Enumerable.Where<PaymentMethodSetting>((IEnumerable<PaymentMethodSetting>)this.Settings, (Func<PaymentMethodSetting, bool>)(s =>
                {
                    long? local_0 = s.LanguageId;
                    long? local_1 = languageId;
                    if (local_0.GetValueOrDefault() == local_1.GetValueOrDefault())
                        return local_0.HasValue == local_1.HasValue;
                    else
                        return false;
                })), (Func<PaymentMethodSetting, string>)(kvp => kvp.Key), (Func<PaymentMethodSetting, string>)(kvp => kvp.Value)))
                    dictionary[keyValuePair.Key] = keyValuePair.Value;
            }
            return (IDictionary<string, string>)dictionary;
        }
    }
}