using System;
using System.Collections.Generic;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Domain.Services;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Models
{
    public class Store : ISortable
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long DefaultCountryId { get; set; }

        public long DefaultVatGroupId { get; set; }

        public long DefaultOrderStatusId { get; set; }

        public long? ConfirmationEmailTemplateId { get; set; }

        public long CurrentCartNumber { get; set; }

        public string OrderNumberPrefix { get; set; }

        public long CurrentOrderNumber { get; set; }

        public string CartNumberPrefix { get; set; }

        public string FirstNamePropertyAlias { get; set; }

        public string LastNamePropertyAlias { get; set; }

        public string EmailPropertyAlias { get; set; }

        public TimeSpan? CookieTimeout { get; set; }

        public string SkuPropertyAlias { get; set; }

        public string NamePropertyAlias { get; set; }

        public string VatGroupPropertyAlias { get; set; }

        public IList<string> ProductPropertyAliases { get; set; }

        public IList<string> ProductUniquenessPropertyAliases { get; set; }

        public long? StockSharingStoreId { get; set; }

        public string EditOrderUiFile { get; set; }

        public IList<string> AllowedFilesForClientRendering { get; set; }

        public int Sort { get; set; }

        public bool IsDeleted { get; set; }

        public Store()
        {
            this.ProductPropertyAliases = (IList<string>)new List<string>();
            this.ProductUniquenessPropertyAliases = (IList<string>)new List<string>();
            this.AllowedFilesForClientRendering = (IList<string>)new List<string>();
            this.Sort = -1;
        }

        public Store(string name)
            : this()
        {
            this.Name = name;
        }

        public void Save()
        {
            bool flag = this.Id == 0L;
            IStoreRepository storeRepository = DependencyResolver.Current.GetService<IStoreRepository>();
            if (this.Sort == -1)
                this.Sort = storeRepository.GetHighestSortValue() + 1;
            storeRepository.Save(this);
            if (this.DefaultCountryId == 0L && this.DefaultVatGroupId == 0L && this.DefaultOrderStatusId == 0L)
            {
                Currency currency = new Currency(this.Id, "JMD", "en-US")
                {
                    IsoCode = "JMD",
                    PricePropertyAlias = "priceJMD"
                };
                currency.Save();
                Country country = new Country(this.Id, "Jamaica", currency.Id)
                {
                    RegionCode = "JM"
                };
                country.Save();
                this.DefaultCountryId = country.Id;
                VatGroup vatGroup = new VatGroup(this.Id, "Default VAT group", new Decimal(0));
                vatGroup.Save();
                this.DefaultVatGroupId = vatGroup.Id;
                OrderStatus orderStatus = new OrderStatus(this.Id, "New");
                orderStatus.Save();
                this.DefaultOrderStatusId = orderStatus.Id;
                new OrderStatus(this.Id, "Completed").Save();
                new OrderStatus(this.Id, "Cancelled").Save();
                //EmailTemplate emailTemplate = new EmailTemplate(this.Id, "Confirmation email")
                //{
                //  SendEmailToCustomer = true
                //};
                //emailTemplate.Settings.Add(new EmailTemplateSettings(new long?())
                //{
                //  Subject = "Tea Commerce confirmation email",
                //  SenderName = "Tea Commerce",
                //  SenderAddress = "info@teacommerce.net"
                //});
                //emailTemplate.Save();
                //this.ConfirmationEmailTemplateId = new long?(emailTemplate.Id);
                PaymentMethod paymentMethod = new PaymentMethod(this.Id, "Credit card")
                {
                    Sku = "4815"
                };
                paymentMethod.AllowedInFollowingCountries.Add(country.Id);
                paymentMethod.PaymentProviderAlias = "Invoicing";
                paymentMethod.Settings.Add(new PaymentMethodSetting("acceptUrl", "/", new long?()));
                paymentMethod.Save();
                ShippingMethod shippingMethod = new ShippingMethod(this.Id, "Pickup")
                {
                    Sku = "1623"
                };
                shippingMethod.AllowedInFollowingCountries.Add(country.Id);
                shippingMethod.Save();
                country.DefaultPaymentMethodId = new long?(paymentMethod.Id);
                country.DefaultShippingMethodId = new long?(shippingMethod.Id);
                country.Save();
                this.CartNumberPrefix = "CART-";
                this.OrderNumberPrefix = "ORDER-";
                this.FirstNamePropertyAlias = "firstName";
                this.LastNamePropertyAlias = "lastName";
                this.EmailPropertyAlias = "email";
                this.SkuPropertyAlias = "sku";
                this.NamePropertyAlias = "productName";
                this.VatGroupPropertyAlias = "vatGroup";
                this.Save();
            }
            if (!flag)
                return;
            NotificationCenter.Store.OnCreated(this);
        }

        public bool Delete()
        {
            this.IsDeleted = true;
            this.Save();
            foreach (Store store in StoreService.Instance.GetAll())
            {
                long? stockSharingStoreId = store.StockSharingStoreId;
                long id = this.Id;
                if ((stockSharingStoreId.GetValueOrDefault() != id ? 0 : (stockSharingStoreId.HasValue ? 1 : 0)) != 0)
                {
                    store.StockSharingStoreId = new long?();
                    store.Save();
                }
            }
            NotificationCenter.Store.OnDeleted(this);
            return true;
        }

        public long GetNextOrderNumber(bool onlyFinalizedOrders)
        {
            long num;
            if (!onlyFinalizedOrders)
            {
                ++this.CurrentCartNumber;
                num = this.CurrentCartNumber;
            }
            else
            {
                ++this.CurrentOrderNumber;
                num = this.CurrentOrderNumber;
            }
            this.Save();
            return num;
        }
    }
}