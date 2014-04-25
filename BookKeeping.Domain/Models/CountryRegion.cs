using System.Collections.Generic;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Domain.Services;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Models
{
    public class CountryRegion : ISortable
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

        public long CountryId
        {
            get;
            set;
        }

        public string Name
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

        public CountryRegion()
        {
            this.Sort = -1;
        }

        public CountryRegion(long storeId, string name, long countryId)
            : this()
        {
            this.StoreId = storeId;
            this.Name = name;
            this.CountryId = countryId;
        }

        public void Save()
        {
            bool flag = this.Id == 0L;
            ICountryRegionRepository countryRegionRepository = DependencyResolver.Current.GetService<ICountryRegionRepository>();
            if (this.Sort == -1)
            {
                this.Sort = countryRegionRepository.GetHighestSortValue(this.StoreId, this.CountryId) + 1;
            }
            countryRegionRepository.Save(this);
            if (this.DefaultShippingMethodId.HasValue && !this.IsDeleted)
            {
                ShippingMethod shippingMethod = ShippingMethodService.Instance.Get(this.StoreId, this.DefaultShippingMethodId.Value);
                if (!shippingMethod.AllowedInFollowingCountryRegions.Contains(this.Id))
                {
                    shippingMethod.AllowedInFollowingCountryRegions.Add(this.Id);
                    shippingMethod.Save();
                }
            }
            if (this.DefaultPaymentMethodId.HasValue && !this.IsDeleted)
            {
                PaymentMethod paymentMethod = PaymentMethodService.Instance.Get(this.StoreId, this.DefaultPaymentMethodId.Value);
                if (!paymentMethod.AllowedInFollowingCountryRegions.Contains(this.Id))
                {
                    paymentMethod.AllowedInFollowingCountryRegions.Add(this.Id);
                    paymentMethod.Save();
                }
            }
            if (flag)
            {
                NotificationCenter.CountryRegion.OnCreated(this);
            }
        }

        public bool Delete()
        {
            this.IsDeleted = true;
            this.Save();
            IEnumerable<ShippingMethod> allAllowedIn = ShippingMethodService.Instance.GetAllAllowedIn(this.StoreId, this.CountryId, new long?(this.Id));
            foreach (ShippingMethod current in allAllowedIn)
            {
                current.AllowedInFollowingCountryRegions.Remove(this.Id);
                current.OriginalPrices.RemoveAll((ServicePrice p) => p.CountryRegionId == this.Id);
                current.Save();
            }
            IEnumerable<PaymentMethod> allAllowedIn2 = PaymentMethodService.Instance.GetAllAllowedIn(this.StoreId, this.CountryId, new long?(this.Id));
            foreach (PaymentMethod current2 in allAllowedIn2)
            {
                current2.AllowedInFollowingCountryRegions.Remove(this.Id);
                current2.OriginalPrices.RemoveAll((ServicePrice p) => p.CountryRegionId == this.Id);
                current2.Save();
            }
            IEnumerable<VatGroup> all = VatGroupService.Instance.GetAll(this.StoreId);
            foreach (VatGroup current3 in all)
            {
                if (current3.CountryRegionSpecificVatRates.ContainsKey(this.Id))
                {
                    current3.CountryRegionSpecificVatRates.Remove(this.Id);
                    current3.Save();
                }
            }
            NotificationCenter.CountryRegion.OnDeleted(this);
            return true;
        }
    }
}