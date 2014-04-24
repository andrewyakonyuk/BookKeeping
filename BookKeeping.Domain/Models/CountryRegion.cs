using System;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Domain.Services;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Models
{
    public class CountryRegion : ISortable
    {
        public long Id { get; set; }

        public long StoreId { get; set; }

        public long CountryId { get; set; }

        public string Name { get; set; }

        public string RegionCode { get; set; }

        public long? DefaultShippingMethodId { get; set; }

        public long? DefaultPaymentMethodId { get; set; }

        public int Sort { get; set; }

        public bool IsDeleted { get; set; }

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
            ICountryRegionRepository regionRepository = DependencyResolver.Current.GetService<ICountryRegionRepository>();
            if (this.Sort == -1)
                this.Sort = regionRepository.GetHighestSortValue(this.StoreId, this.CountryId) + 1;
            regionRepository.Save(this);
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
            if (!flag)
                return;
            NotificationCenter.CountryRegion.OnCreated(this);
        }

        public bool Delete()
        {
            this.IsDeleted = true;
            this.Save();
            foreach (ShippingMethod shippingMethod in ShippingMethodService.Instance.GetAllAllowedIn(this.StoreId, this.CountryId, new long?(this.Id)))
            {
                shippingMethod.AllowedInFollowingCountryRegions.Remove(this.Id);
                shippingMethod.OriginalPrices.RemoveAll((Predicate<ServicePrice>)(p =>
                {
                    long? local_0 = p.CountryRegionId;
                    long local_1 = this.Id;
                    if (local_0.GetValueOrDefault() == local_1)
                        return local_0.HasValue;
                    else
                        return false;
                }));
                shippingMethod.Save();
            }
            foreach (PaymentMethod paymentMethod in PaymentMethodService.Instance.GetAllAllowedIn(this.StoreId, this.CountryId, new long?(this.Id)))
            {
                paymentMethod.AllowedInFollowingCountryRegions.Remove(this.Id);
                paymentMethod.OriginalPrices.RemoveAll((Predicate<ServicePrice>)(p =>
                {
                    long? local_0 = p.CountryRegionId;
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
                if (vatGroup.CountryRegionSpecificVatRates.ContainsKey(this.Id))
                {
                    vatGroup.CountryRegionSpecificVatRates.Remove(this.Id);
                    vatGroup.Save();
                }
            }
            NotificationCenter.CountryRegion.OnDeleted(this);
            return true;
        }
    }
}