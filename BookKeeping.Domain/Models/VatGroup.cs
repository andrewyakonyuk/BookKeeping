using System.Collections.Generic;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Domain.Services;
using BookKeeping.Infrastructure.Dependency;
using BookKeeping.Infrastructure.Domain;
using BookKeeping.Infrastructure.Logging;

namespace BookKeeping.Domain.Models
{
    public class VatGroup : ISortable, IEntity
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

        public VatRate DefaultVatRate
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

        public IDictionary<long, VatRate> CountrySpecificVatRates
        {
            get;
            set;
        }

        public IDictionary<long, VatRate> CountryRegionSpecificVatRates
        {
            get;
            set;
        }

        public VatGroup()
        {
            this.DefaultVatRate = new VatRate();
            this.Sort = -1;
            this.CountrySpecificVatRates = new Dictionary<long, VatRate>();
            this.CountryRegionSpecificVatRates = new Dictionary<long, VatRate>();
        }

        public VatGroup(long storeId, string name, decimal defaultVatRate)
            : this()
        {
            this.StoreId = storeId;
            this.Name = name;
            this.DefaultVatRate = new VatRate(defaultVatRate);
        }

        public void Save()
        {
            bool flag = this.Id == 0L;
            IVatGroupRepository vatGroupRepository = DependencyResolver.Current.GetService<IVatGroupRepository>();
            if (this.Sort == -1)
            {
                this.Sort = vatGroupRepository.GetHighestSortValue(this.StoreId) + 1;
            }
            vatGroupRepository.Save(this);
            if (flag)
            {
                NotificationCenter.VatGroup.OnCreated(this);
            }
        }

        public bool Delete()
        {
            bool result = false;
            Store store = StoreService.Instance.Get(this.StoreId);
            if (store.DefaultVatGroupId != this.Id)
            {
                foreach (ShippingMethod current in ShippingMethodService.Instance.GetAll(this.StoreId))
                {
                    if (current.VatGroupId == this.Id)
                    {
                        current.VatGroupId = null;
                        current.Save();
                    }
                }
                foreach (PaymentMethod current2 in PaymentMethodService.Instance.GetAll(this.StoreId))
                {
                    if (current2.VatGroupId == this.Id)
                    {
                        current2.VatGroupId = null;
                        current2.Save();
                    }
                }
                this.IsDeleted = true;
                this.Save();
                result = true;
                NotificationCenter.VatGroup.OnDeleted(this);
            }
            else
            {
                LoggingService.Instance.Log("Can't delete the vat group " + this.Name + " because it is the default for the store " + store.Name);
            }
            return result;
        }

        public VatRate GetVatRate(long countryId, long? countryRegionId)
        {
            VatRate vatRate = null;
            if (countryRegionId.HasValue && this.CountryRegionSpecificVatRates.ContainsKey(countryRegionId.Value))
            {
                vatRate = this.CountryRegionSpecificVatRates[countryRegionId.Value];
            }
            if (vatRate == null && this.CountrySpecificVatRates.ContainsKey(countryId))
            {
                vatRate = this.CountrySpecificVatRates[countryId];
            }
            return vatRate ?? this.DefaultVatRate;
        }
    }
}