using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Domain.Services;
using BookKeeping.Infrastructure.Dependency;
using BookKeeping.Infrastructure.Domain;
using BookKeeping.Infrastructure.Logging;

namespace BookKeeping.Domain.Models
{
    public class Currency : ISortable, IEntity
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

        public string IsoCode
        {
            get;
            set;
        }

        public string PricePropertyAlias
        {
            get;
            set;
        }

        public string CultureName
        {
            get;
            set;
        }

        public string Symbol
        {
            get;
            set;
        }

        public CurrencySymbolPlacement? SymbolPlacement
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

        public IList<long> AllowedInFollowingCountries
        {
            get;
            set;
        }

        public Currency()
        {
            this.AllowedInFollowingCountries = new List<long>();
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
                Currency currency = DependencyResolver.Current.GetService<ICurrencyRepository>().Get(this.StoreId, this.Id);
                List<long> removedAllowedCountries = (
                    from i in currency.AllowedInFollowingCountries
                    where !this.AllowedInFollowingCountries.Contains(i)
                    select i).ToList<long>();
                if (removedAllowedCountries.Any<long>())
                {
                    foreach (long current in new List<long>(removedAllowedCountries))
                    {
                        Country country = CountryService.Instance.Get(this.StoreId, current);
                        if (country != null && country.DefaultCurrencyId == this.Id)
                        {
                            removedAllowedCountries.Remove(current);
                            this.AllowedInFollowingCountries.Add(current);
                        }
                    }
                    foreach (ShippingMethod current2 in ShippingMethodService.Instance.GetAll(this.StoreId))
                    {
                        if (current2.OriginalPrices.RemoveAll((ServicePrice p) => p.CurrencyId == this.Id && p.CountryId.HasValue && removedAllowedCountries.Contains(p.CountryId.Value)) > 0)
                        {
                            current2.Save();
                        }
                    }
                    foreach (PaymentMethod current3 in PaymentMethodService.Instance.GetAll(this.StoreId))
                    {
                        if (current3.OriginalPrices.RemoveAll((ServicePrice p) => p.CurrencyId == this.Id && p.CountryId.HasValue && removedAllowedCountries.Contains(p.CountryId.Value)) > 0)
                        {
                            current3.Save();
                        }
                    }
                }
            }
            ICurrencyRepository currencyRepository = DependencyResolver.Current.GetService<ICurrencyRepository>();
            if (this.Sort == -1)
            {
                this.Sort = currencyRepository.GetHighestSortValue(this.StoreId) + 1;
            }
            currencyRepository.Save(this);
            if (flag)
            {
                NotificationCenter.Currency.OnCreated(this);
            }
        }

        public bool Delete()
        {
            bool result = false;
            Country country = CountryService.Instance.GetAll(this.StoreId).FirstOrDefault((Country c) => c.DefaultCurrencyId == this.Id);
            if (country == null)
            {
                this.IsDeleted = true;
                this.Save();
                result = true;
                foreach (ShippingMethod current in ShippingMethodService.Instance.GetAll(this.StoreId))
                {
                    if (current.OriginalPrices.RemoveAll((ServicePrice p) => p.CurrencyId == this.Id) > 0)
                    {
                        current.Save();
                    }
                }
                foreach (PaymentMethod current2 in PaymentMethodService.Instance.GetAll(this.StoreId))
                {
                    if (current2.OriginalPrices.RemoveAll((ServicePrice p) => p.CurrencyId == this.Id) > 0)
                    {
                        current2.Save();
                    }
                }
                NotificationCenter.Currency.OnDeleted(this);
            }
            else
            {
                LoggingService.Instance.Log("Can't delete the currency " + this.Name + " because it is the default for the country " + country.Name);
            }
            return result;
        }

        public bool IsAllowedInCountry(long countryId)
        {
            return !this.IsDeleted && this.AllowedInFollowingCountries.Contains(countryId);
        }

        public string FormatMoney(decimal money)
        {
            string text;
            if (!string.IsNullOrEmpty(this.Symbol))
            {
                text = this.FormatMoneyWithoutSymbol(money);
                if (this.SymbolPlacement.HasValue && this.SymbolPlacement.Value == CurrencySymbolPlacement.Right)
                {
                    text += this.Symbol;
                }
                else
                {
                    text = this.Symbol + text;
                }
            }
            else
            {
                text = money.ToString("c", CultureInfo.GetCultureInfo(this.CultureName));
            }
            return text;
        }

        public string FormatMoneyWithoutSymbol(decimal money)
        {
            CultureInfo cultureInfo = CultureInfo.GetCultureInfo(this.CultureName);
            int[] array = cultureInfo.NumberFormat.CurrencyGroupSizes;
            int currencyDecimalDigits = cultureInfo.NumberFormat.CurrencyDecimalDigits;
            array = Enumerable.Repeat<int>(array.First<int>(), 4).Concat(array).Reverse<int>().ToArray<int>();
            string text = new string('0', currencyDecimalDigits);
            text = "0." + text;
            for (int i = 0; i < array.Length; i++)
            {
                if (i == 0)
                {
                    text = new string('#', array[i] - 1) + text;
                }
                else
                {
                    text = ',' + text;
                    text = new string('#', array[i]) + text;
                }
            }
            return money.ToString(text, cultureInfo);
        }
    }
}