using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Domain.Services;
using BookKeeping.Infrastructure.Dependency;
using BookKeeping.Infrastructure.Logging;

namespace BookKeeping.Domain.Models
{
    public class Currency : ISortable
    {
        public long Id { get; set; }

        public long StoreId { get; set; }

        public string Name { get; set; }

        public string IsoCode { get; set; }

        public string PricePropertyAlias { get; set; }

        public string CultureName { get; set; }

        public string Symbol { get; set; }

        public CurrencySymbolPlacement? SymbolPlacement { get; set; }

        public int Sort { get; set; }

        public bool IsDeleted { get; set; }

        public IList<long> AllowedInFollowingCountries { get; set; }

        public Currency()
        {
            this.AllowedInFollowingCountries = (IList<long>)new List<long>();
            this.Sort = -1;
        }

        public Currency(long storeId, string name, string cultureName)
            : this()
        {
            this.StoreId = storeId;
            this.Name = name;
            this.CultureName = cultureName;
        }

        public void Save()
        {
            bool flag = this.Id == 0L;
            if (!flag)
            {
                var removedAllowedCountries = DependencyResolver.Current.GetService<ICurrencyRepository>().Get(this.StoreId, this.Id).AllowedInFollowingCountries.Where(i => !this.AllowedInFollowingCountries.Contains(i)).ToList();
                if (removedAllowedCountries.Any())
                {
                    foreach (long countryId in removedAllowedCountries)
                    {
                        Country country = CountryService.Instance.Get(this.StoreId, countryId);
                        if (country != null && country.DefaultCurrencyId == this.Id)
                        {
                            removedAllowedCountries.Remove(countryId);
                            this.AllowedInFollowingCountries.Add(countryId);
                        }
                    }
                    foreach (ShippingMethod shippingMethod in ShippingMethodService.Instance.GetAll(this.StoreId))
                    {
                        if (shippingMethod.OriginalPrices.RemoveAll(p =>
                        {
                            if (p.CurrencyId == this.Id && p.CountryId.HasValue)
                                return removedAllowedCountries.Contains(p.CountryId.Value);
                            else
                                return false;
                        }) > 0)
                        {
                            shippingMethod.Save();
                        }
                    }
                    foreach (PaymentMethod paymentMethod in PaymentMethodService.Instance.GetAll(this.StoreId))
                    {
                        if (paymentMethod.OriginalPrices.RemoveAll((Predicate<ServicePrice>)(p =>
                        {
                            if (p.CurrencyId == this.Id && p.CountryId.HasValue)
                                return removedAllowedCountries.Contains(p.CountryId.Value);
                            else
                                return false;
                        })) > 0)
                        {
                            paymentMethod.Save();
                        }
                    }
                }
            }
            ICurrencyRepository currencyRepository = DependencyResolver.Current.GetService<ICurrencyRepository>();
            if (this.Sort == -1)
                this.Sort = currencyRepository.GetHighestSortValue(this.StoreId) + 1;
            currencyRepository.Save(this);
            if (!flag)
                return;
            NotificationCenter.Currency.OnCreated(this);
        }

        public bool Delete()
        {
            bool flag = false;
            Country country = CountryService.Instance.GetAll(this.StoreId).FirstOrDefault(c => c.DefaultCurrencyId == this.Id);
            if (country == null)
            {
                this.IsDeleted = true;
                this.Save();
                flag = true;
                foreach (ShippingMethod shippingMethod in ShippingMethodService.Instance.GetAll(this.StoreId))
                {
                    if (shippingMethod.OriginalPrices.RemoveAll(p => p.CurrencyId == this.Id) > 0)
                        shippingMethod.Save();
                }
                foreach (PaymentMethod paymentMethod in PaymentMethodService.Instance.GetAll(this.StoreId))
                {
                    if (paymentMethod.OriginalPrices.RemoveAll(p => p.CurrencyId == this.Id) > 0)
                        paymentMethod.Save();
                }
                NotificationCenter.Currency.OnDeleted(this);
            }
            else
                LoggingService.Instance.Log("Can't delete the currency " + this.Name + " because it is the default for the country " + country.Name);
            return flag;
        }

        public bool IsAllowedInCountry(long countryId)
        {
            return !this.IsDeleted && this.AllowedInFollowingCountries.Contains(countryId);
        }

        public string FormatMoney(Decimal money)
        {
            string formated;
            if (!string.IsNullOrEmpty(this.Symbol))
            {
                string withoutSymbol = this.FormatMoneyWithoutSymbol(money);
                formated = !this.SymbolPlacement.HasValue || this.SymbolPlacement.Value != CurrencySymbolPlacement.Right ? this.Symbol + withoutSymbol : withoutSymbol + this.Symbol;
            }
            else
                formated = money.ToString("c", CultureInfo.GetCultureInfo(this.CultureName));
            return formated;
        }

        public string FormatMoneyWithoutSymbol(Decimal money)
        {
            CultureInfo cultureInfo = CultureInfo.GetCultureInfo(this.CultureName);
            int[] currencyGroupSizes = cultureInfo.NumberFormat.CurrencyGroupSizes;
            int currencyDecimalDigits = cultureInfo.NumberFormat.CurrencyDecimalDigits;
            int[] numArray = Enumerable.ToArray<int>(Enumerable.Reverse<int>(Enumerable.Concat<int>(Enumerable.Repeat<int>(Enumerable.First<int>(currencyGroupSizes), 4), currencyGroupSizes)));
            string format = "0." + new string('0', currencyDecimalDigits);
            for (int index = 0; index < numArray.Length; ++index)
            {
                if (index == 0)
                {
                    format = new string('#', numArray[index] - 1) + format;
                }
                else
                {
                    string str = (string)(object)',' + (object)format;
                    format = new string('#', numArray[index]) + str;
                }
            }
            return money.ToString(format, (IFormatProvider)cultureInfo);
        }
    }
}