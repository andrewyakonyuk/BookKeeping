using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.PriceCalculators;
using BookKeeping.Domain.Repositories;
using BookKeeping.Domain.Services;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Models
{
    public class Order : ICopyable<Order>
    {
        public Guid Id { get; set; }

        public long StoreId { get; set; }

        public Guid? CopiedFromOrderId { get; set; }

        public long CurrencyId { get; set; }

        public long OrderStatusId { get; set; }

        public long VatGroupId { get; set; }

        public long? LanguageId { get; set; }

        public string CartNumber { get; set; }

        public string OrderNumber { get; set; }

        public string CustomerId { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public DateTime? DateFinalized { get; set; }

        public bool IsFinalized
        {
            get
            {
                return this.DateFinalized.HasValue;
            }
        }

        public Decimal TotalQuantity
        {
            get
            {
                return Enumerable.Sum<OrderLine>((IEnumerable<OrderLine>)this.OrderLines, (Func<OrderLine, Decimal>)(ol => ol.Quantity));
            }
        }

        public OrderLineCollection OrderLines { get; set; }

        public CustomPropertyCollection Properties { get; set; }

        public PaymentInformation PaymentInformation { get; set; }

        public ShipmentInformation ShipmentInformation { get; set; }

        public TransactionInformation TransactionInformation { get; set; }

        public VatRate VatRate { get; set; }

        public PriceCollection SubtotalPrices { get; set; }

        public Price SubtotalPrice { get; set; }

        public PriceCollection TotalPrices { get; set; }

        public Price TotalPrice { get; set; }

        public Order()
        {
            this.OrderLines = new OrderLineCollection();
            this.Properties = new CustomPropertyCollection();
            this.PaymentInformation = new PaymentInformation();
            this.ShipmentInformation = new ShipmentInformation();
            this.TransactionInformation = new TransactionInformation();
            this.VatRate = new VatRate();
            this.SubtotalPrices = new PriceCollection();
            this.SubtotalPrice = new Price();
            this.TotalPrices = new PriceCollection();
            this.TotalPrice = new Price();
        }

        public Order(long storeId)
            : this()
        {
            Store store = StoreService.Instance.Get(storeId);
            Country country = CountryService.Instance.Get(storeId, store.DefaultCountryId);
            this.StoreId = storeId;
            this.CurrencyId = country.DefaultCurrencyId;
            this.OrderStatusId = store.DefaultOrderStatusId;
            this.VatGroupId = store.DefaultVatGroupId;
            this.PaymentInformation.CountryId = country.Id;
            this.PaymentInformation.PaymentMethodId = country.DefaultPaymentMethodId;
            this.ShipmentInformation.ShippingMethodId = country.DefaultShippingMethodId;
        }

        public void Save()
        {
            Store store = StoreService.Instance.Get(this.StoreId);
            if (this.Id == Guid.Empty)
            {
                this.DateCreated = DateTime.Now;
                this.CartNumber = store.CartNumberPrefix + (object)store.GetNextOrderNumber(false);
            }
            this.AutoSetCustomerInformation(store);
            this.RemoveEmptyOrderLines((List<OrderLine>)this.OrderLines);
            DependencyResolver.Current.GetService<IOrderCalculator>().CalculateOrder(this);
            IOrderRepository orderRepository = DependencyResolver.Current.GetService<IOrderRepository>();
            Order orderToCompare = orderRepository.Get(this.StoreId, this.Id) ?? new Order();
            if (this.Equals((object)orderToCompare))
                return;
            this.DateModified = DateTime.Now;
            this.CompareAndNotifyBeforeSave(orderToCompare);
            this.AutoSetCustomerInformation(store);
            this.RemoveEmptyOrderLines((List<OrderLine>)this.OrderLines);
            orderRepository.Save(this);
            this.CompareAndNotifyAfterSave(orderToCompare);
        }

        private void AutoSetCustomerInformation(Store store)
        {
            CustomProperty customProperty1 = this.Properties.Get(store.FirstNamePropertyAlias);
            if (customProperty1 != null)
                this.PaymentInformation.FirstName = customProperty1.Value;
            CustomProperty customProperty2 = this.Properties.Get(store.LastNamePropertyAlias);
            if (customProperty2 != null)
                this.PaymentInformation.LastName = customProperty2.Value;
            CustomProperty customProperty3 = this.Properties.Get(store.EmailPropertyAlias);
            if (customProperty3 == null)
                return;
            this.PaymentInformation.Email = customProperty3.Value;
        }

        private void RemoveEmptyOrderLines(List<OrderLine> orderLines)
        {
            orderLines.RemoveAll((Predicate<OrderLine>)(ol => ol.Quantity <= new Decimal(0)));
            foreach (OrderLine orderLine in orderLines)
                this.RemoveEmptyOrderLines((List<OrderLine>)orderLine.OrderLines);
        }

        public bool Delete()
        {
            NotificationCenter.Order.OnDeleting(this);
            DependencyResolver.Current.GetService<IOrderRepository>().Delete(this);
            NotificationCenter.Order.OnDeleted(this);
            return true;
        }

        public bool ChangeCurrency(long currencyId)
        {
            Currency currency = CurrencyService.Instance.Get(this.StoreId, currencyId);
            bool flag = currency != null && currency.IsAllowedInCountry(this.PaymentInformation.CountryId);
            if (flag)
                this.CurrencyId = currencyId;
            return flag;
        }

        public bool ChangePaymentRegion(long countryId, long? countryRegionId = null)
        {
            Country country = CountryService.Instance.Get(this.StoreId, countryId);
            CountryRegion countryRegion = countryRegionId.HasValue ? CountryRegionService.Instance.Get(this.StoreId, countryRegionId.Value) : (CountryRegion)null;
            bool flag = country != null && !country.IsDeleted && (!countryRegionId.HasValue || countryRegion != null && !countryRegion.IsDeleted);
            if (flag)
            {
                this.PaymentInformation.CountryId = countryId;
                this.PaymentInformation.CountryRegionId = countryRegionId;
                Currency currency = CurrencyService.Instance.Get(this.StoreId, this.CurrencyId);
                if (currency == null || !currency.IsAllowedInCountry(this.PaymentInformation.CountryId))
                    this.CurrencyId = country.DefaultCurrencyId;
                if (this.PaymentInformation.PaymentMethodId.HasValue)
                {
                    PaymentMethod paymentMethod = PaymentMethodService.Instance.Get(this.StoreId, this.PaymentInformation.PaymentMethodId.Value);
                    if (paymentMethod == null || !paymentMethod.IsAllowedInRegion(this.PaymentInformation.CountryId, this.PaymentInformation.CountryRegionId))
                        this.PaymentInformation.PaymentMethodId = !this.PaymentInformation.CountryRegionId.HasValue || countryRegion == null ? country.DefaultPaymentMethodId : countryRegion.DefaultPaymentMethodId;
                }
                if (this.ShipmentInformation.ShippingMethodId.HasValue)
                {
                    ShippingMethod shippingMethod = ShippingMethodService.Instance.Get(this.StoreId, this.ShipmentInformation.ShippingMethodId.Value);
                    if (shippingMethod == null || !shippingMethod.IsAllowedInRegion(this.ShipmentInformation.CountryId ?? this.PaymentInformation.CountryId, this.ShipmentInformation.CountryId.HasValue ? this.ShipmentInformation.CountryRegionId : this.PaymentInformation.CountryRegionId))
                        this.ShipmentInformation.ShippingMethodId = new long?();
                }
            }
            return flag;
        }

        public bool ChangeShippingRegion(long? countryId, long? countryRegionId = null)
        {
            Country country = countryId.HasValue ? CountryService.Instance.Get(this.StoreId, countryId.Value) : (Country)null;
            CountryRegion countryRegion = countryRegionId.HasValue ? CountryRegionService.Instance.Get(this.StoreId, countryRegionId.Value) : (CountryRegion)null;
            bool flag = !countryId.HasValue && !countryRegionId.HasValue || country != null && !country.IsDeleted && (!countryRegionId.HasValue || countryRegion != null && !countryRegion.IsDeleted);
            if (flag)
            {
                this.ShipmentInformation.CountryId = countryId;
                this.ShipmentInformation.CountryRegionId = countryRegionId;
                if (this.ShipmentInformation.ShippingMethodId.HasValue)
                {
                    ShippingMethod shippingMethod = ShippingMethodService.Instance.Get(this.StoreId, this.ShipmentInformation.ShippingMethodId.Value);
                    if (shippingMethod == null || !shippingMethod.IsAllowedInRegion(this.ShipmentInformation.CountryId ?? this.PaymentInformation.CountryId, this.ShipmentInformation.CountryId.HasValue ? this.ShipmentInformation.CountryRegionId : this.PaymentInformation.CountryRegionId))
                        this.ShipmentInformation.ShippingMethodId = new long?();
                }
            }
            return flag;
        }

        public bool ChangePaymentMethod(long? paymentMethodId)
        {
            bool flag = !paymentMethodId.HasValue;
            if (paymentMethodId.HasValue)
            {
                PaymentMethod paymentMethod = PaymentMethodService.Instance.Get(this.StoreId, paymentMethodId.Value);
                flag = paymentMethod != null && paymentMethod.IsAllowedInRegion(this.PaymentInformation.CountryId, this.PaymentInformation.CountryRegionId);
            }
            if (flag)
                this.PaymentInformation.PaymentMethodId = paymentMethodId;
            return flag;
        }

        public bool ChangeShippingMethod(long? shippingMethodId)
        {
            bool flag = !shippingMethodId.HasValue;
            if (shippingMethodId.HasValue)
            {
                ShippingMethod shippingMethod = ShippingMethodService.Instance.Get(this.StoreId, shippingMethodId.Value);
                flag = shippingMethod != null && shippingMethod.IsAllowedInRegion(this.ShipmentInformation.CountryId ?? this.PaymentInformation.CountryId, this.ShipmentInformation.CountryId.HasValue ? this.ShipmentInformation.CountryRegionId : this.PaymentInformation.CountryRegionId);
            }
            if (flag)
                this.ShipmentInformation.ShippingMethodId = shippingMethodId;
            return flag;
        }

        public void Finalize(Decimal amountAuthorized, string transactionId, PaymentState paymentState, string paymentType = null, string paymentIdentifier = null)
        {
            if (this.IsFinalized)
                return;
            Store store = StoreService.Instance.Get(this.StoreId);
            this.DateFinalized = new DateTime?(DateTime.Now);
            this.OrderNumber = store.OrderNumberPrefix + (object)store.GetNextOrderNumber(true);
            Currency currency = CurrencyService.Instance.Get(this.StoreId, this.CurrencyId);
            this.TransactionInformation.AmountAuthorized = new PriceWithoutVat(amountAuthorized, currency);
            this.TransactionInformation.TransactionFee = new PriceWithoutVat(amountAuthorized - this.TotalPrice.WithVat, currency);
            this.TransactionInformation.TransactionId = transactionId;
            this.TransactionInformation.PaymentState = new PaymentState?(paymentState);
            this.TransactionInformation.PaymentType = paymentType;
            this.TransactionInformation.PaymentIdentifier = paymentIdentifier;
            this.Save();
            this.RemoveItemsFromStock((IEnumerable<OrderLine>)this.OrderLines, new Decimal(1));
            if (!store.ConfirmationEmailTemplateId.HasValue)
                return;
            //EmailTemplate emailTemplate = EmailTemplateService.Instance.Get(store.Id, store.ConfirmationEmailTemplateId.Value);
            //if (emailTemplate == null)
            //  return;
            //emailTemplate.Send(this);
        }

        private void RemoveItemsFromStock(IEnumerable<OrderLine> orderLines, Decimal parentOrderLineQuantity)
        {
            foreach (OrderLine orderLine in orderLines)
            {
                this.RemoveItemsFromStock((IEnumerable<OrderLine>)orderLine.OrderLines, orderLine.Quantity * parentOrderLineQuantity);
                Decimal? stock = ProductService.Instance.GetStock(this.StoreId, orderLine.Sku);
                if (stock.HasValue)
                    ProductService.Instance.SetStock(this.StoreId, orderLine.Sku, new Decimal?(stock.Value - orderLine.Quantity * parentOrderLineQuantity));
            }
        }

        private void CompareAndNotifyBeforeSave(Order orderToCompare)
        {
            Contract.Requires<ArgumentNullException>(orderToCompare != null, "orderToCompare");
            if (orderToCompare.Id == Guid.Empty)
                NotificationCenter.Order.OnCreating(this);
            NotificationCenter.Order.OnUpdating(this);
            long? languageId1 = this.LanguageId;
            long? languageId2 = orderToCompare.LanguageId;
            if ((languageId1.GetValueOrDefault() != languageId2.GetValueOrDefault() ? 1 : (languageId1.HasValue != languageId2.HasValue ? 1 : 0)) != 0)
                NotificationCenter.Order.OnLanguageChanging(this);
            if (this.CustomerId != orderToCompare.CustomerId)
                NotificationCenter.Order.OnCustomerChanging(this);
            if (this.VatGroupId != orderToCompare.VatGroupId)
                NotificationCenter.Order.OnVatGroupChanging(this);
            if (this.CurrencyId != orderToCompare.CurrencyId)
                NotificationCenter.Order.OnCurrencyChanging(this);
            List<OrderLine> allOrderLines = Enumerable.ToList<OrderLine>(this.OrderLines.GetAll());
            List<OrderLine> orderToCompareAllOrderLines = Enumerable.ToList<OrderLine>(orderToCompare.OrderLines.GetAll());
            IEnumerable<OrderLine> enumerable1 = (IEnumerable<OrderLine>)Enumerable.ToList<OrderLine>(Enumerable.Where<OrderLine>((IEnumerable<OrderLine>)allOrderLines, (Func<OrderLine, bool>)(ol => Enumerable.All<OrderLine>((IEnumerable<OrderLine>)orderToCompareAllOrderLines, (Func<OrderLine, bool>)(col => col.Id != ol.Id)))));
            if (Enumerable.Any<OrderLine>(enumerable1))
                NotificationCenter.Order.OnOrderLinesAdding(this, enumerable1);
            IEnumerable<OrderLine> enumerable2 = (IEnumerable<OrderLine>)Enumerable.ToList<OrderLine>(Enumerable.Concat<OrderLine>(Enumerable.Where<OrderLine>((IEnumerable<OrderLine>)allOrderLines, (Func<OrderLine, bool>)(ol => Enumerable.Any<OrderLine>((IEnumerable<OrderLine>)orderToCompareAllOrderLines, (Func<OrderLine, bool>)(col =>
            {
                if (col.Id == ol.Id)
                    return !col.Equals((object)ol);
                else
                    return false;
            })))), enumerable1));
            if (Enumerable.Any<OrderLine>(enumerable2))
                NotificationCenter.Order.OnOrderLinesUpdating(this, enumerable2);
            IList<OrderLine> list = (IList<OrderLine>)new List<OrderLine>();
            foreach (OrderLine orderLine1 in Enumerable.Where<OrderLine>((IEnumerable<OrderLine>)orderToCompareAllOrderLines, (Func<OrderLine, bool>)(col => Enumerable.All<OrderLine>((IEnumerable<OrderLine>)allOrderLines, (Func<OrderLine, bool>)(ol => ol.Id != col.Id)))))
            {
                OrderLine orderLine2 = orderLine1.Copy();
                orderLine2.Id = orderLine1.Id;
                list.Add(orderLine2);
            }
            if (Enumerable.Any<OrderLine>((IEnumerable<OrderLine>)list))
                NotificationCenter.Order.OnOrderLinesRemoving(this, (IEnumerable<OrderLine>)list);
            IEnumerable<CustomProperty> enumerable3 = (IEnumerable<CustomProperty>)Enumerable.ToList<CustomProperty>(Enumerable.Where<CustomProperty>((IEnumerable<CustomProperty>)this.Properties, (Func<CustomProperty, bool>)(p => Enumerable.All<CustomProperty>((IEnumerable<CustomProperty>)orderToCompare.Properties, (Func<CustomProperty, bool>)(cp => cp.Alias != p.Alias)))));
            if (Enumerable.Any<CustomProperty>(enumerable3))
                NotificationCenter.Order.OnOrderPropertiesAdding(this, enumerable3);
            IEnumerable<CustomProperty> enumerable4 = (IEnumerable<CustomProperty>)Enumerable.ToList<CustomProperty>(Enumerable.Concat<CustomProperty>(Enumerable.Where<CustomProperty>((IEnumerable<CustomProperty>)this.Properties, (Func<CustomProperty, bool>)(p => Enumerable.Any<CustomProperty>((IEnumerable<CustomProperty>)orderToCompare.Properties, (Func<CustomProperty, bool>)(cp =>
            {
                if (cp.Alias == p.Alias)
                    return !cp.Equals((object)p);
                else
                    return false;
            })))), enumerable3));
            if (Enumerable.Any<CustomProperty>(enumerable4))
                NotificationCenter.Order.OnOrderPropertiesUpdating(this, enumerable4);
            IEnumerable<CustomProperty> enumerable5 = (IEnumerable<CustomProperty>)Enumerable.ToList<CustomProperty>(Enumerable.Select<CustomProperty, CustomProperty>(Enumerable.Where<CustomProperty>((IEnumerable<CustomProperty>)orderToCompare.Properties, (Func<CustomProperty, bool>)(cp => Enumerable.All<CustomProperty>((IEnumerable<CustomProperty>)this.Properties, (Func<CustomProperty, bool>)(p => p.Alias != cp.Alias)))), (Func<CustomProperty, CustomProperty>)(cp => cp.Copy())));
            if (Enumerable.Any<CustomProperty>(enumerable5))
                NotificationCenter.Order.OnOrderPropertiesRemoving(this, enumerable5);
            if (this.PaymentInformation.CountryId != orderToCompare.PaymentInformation.CountryId)
                NotificationCenter.Order.OnPaymentCountryChanging(this);
            long? countryRegionId1 = this.PaymentInformation.CountryRegionId;
            long? countryRegionId2 = orderToCompare.PaymentInformation.CountryRegionId;
            if ((countryRegionId1.GetValueOrDefault() != countryRegionId2.GetValueOrDefault() ? 1 : (countryRegionId1.HasValue != countryRegionId2.HasValue ? 1 : 0)) != 0)
                NotificationCenter.Order.OnPaymentCountryRegionChanging(this);
            long? paymentMethodId1 = this.PaymentInformation.PaymentMethodId;
            long? paymentMethodId2 = orderToCompare.PaymentInformation.PaymentMethodId;
            if ((paymentMethodId1.GetValueOrDefault() != paymentMethodId2.GetValueOrDefault() ? 1 : (paymentMethodId1.HasValue != paymentMethodId2.HasValue ? 1 : 0)) != 0)
                NotificationCenter.Order.OnPaymentMethodChanging(this);
            long? countryId1 = this.ShipmentInformation.CountryId;
            long? countryId2 = orderToCompare.ShipmentInformation.CountryId;
            if ((countryId1.GetValueOrDefault() != countryId2.GetValueOrDefault() ? 1 : (countryId1.HasValue != countryId2.HasValue ? 1 : 0)) != 0)
                NotificationCenter.Order.OnShippingCountryChanging(this);
            long? countryRegionId3 = this.ShipmentInformation.CountryRegionId;
            long? countryRegionId4 = orderToCompare.ShipmentInformation.CountryRegionId;
            if ((countryRegionId3.GetValueOrDefault() != countryRegionId4.GetValueOrDefault() ? 1 : (countryRegionId3.HasValue != countryRegionId4.HasValue ? 1 : 0)) != 0)
                NotificationCenter.Order.OnShippingCountryRegionChanging(this);
            long? shippingMethodId1 = this.ShipmentInformation.ShippingMethodId;
            long? shippingMethodId2 = orderToCompare.ShipmentInformation.ShippingMethodId;
            if ((shippingMethodId1.GetValueOrDefault() != shippingMethodId2.GetValueOrDefault() ? 1 : (shippingMethodId1.HasValue != shippingMethodId2.HasValue ? 1 : 0)) != 0)
                NotificationCenter.Order.OnShippingMethodChanging(this);
            PaymentState? paymentState1 = this.TransactionInformation.PaymentState;
            PaymentState? paymentState2 = orderToCompare.TransactionInformation.PaymentState;
            if ((paymentState1.GetValueOrDefault() != paymentState2.GetValueOrDefault() ? 1 : (paymentState1.HasValue != paymentState2.HasValue ? 1 : 0)) != 0)
                NotificationCenter.Order.OnPaymentStateChanging(this);
            if (this.OrderStatusId != orderToCompare.OrderStatusId)
                NotificationCenter.Order.OnOrderStatusChanging(this);
            if (!this.IsFinalized || orderToCompare.IsFinalized)
                return;
            NotificationCenter.Order.OnFinalizing(this);
        }

        private void CompareAndNotifyAfterSave(Order orderToCompare)
        {
            Contract.Requires<ArgumentNullException>(orderToCompare != null, "orderToCompare");
            if (orderToCompare.Id == Guid.Empty)
                NotificationCenter.Order.OnCreated(this);
            NotificationCenter.Order.OnUpdated(this);
            long? languageId1 = this.LanguageId;
            long? languageId2 = orderToCompare.LanguageId;
            if ((languageId1.GetValueOrDefault() != languageId2.GetValueOrDefault() ? 1 : (languageId1.HasValue != languageId2.HasValue ? 1 : 0)) != 0)
                NotificationCenter.Order.OnLanguageChanged(this);
            if (this.CustomerId != orderToCompare.CustomerId)
                NotificationCenter.Order.OnCustomerChanged(this);
            if (this.VatGroupId != orderToCompare.VatGroupId)
                NotificationCenter.Order.OnVatGroupChanged(this);
            if (this.CurrencyId != orderToCompare.CurrencyId)
                NotificationCenter.Order.OnCurrencyChanged(this);
            List<OrderLine> allOrderLines = Enumerable.ToList<OrderLine>(this.OrderLines.GetAll());
            List<OrderLine> orderToCompareAllOrderLines = Enumerable.ToList<OrderLine>(orderToCompare.OrderLines.GetAll());
            IEnumerable<OrderLine> enumerable1 = (IEnumerable<OrderLine>)Enumerable.ToList<OrderLine>(Enumerable.Where<OrderLine>((IEnumerable<OrderLine>)allOrderLines, (Func<OrderLine, bool>)(ol => Enumerable.All<OrderLine>((IEnumerable<OrderLine>)orderToCompareAllOrderLines, (Func<OrderLine, bool>)(col => col.Id != ol.Id)))));
            if (Enumerable.Any<OrderLine>(enumerable1))
                NotificationCenter.Order.OnOrderLineAdded(this, enumerable1);
            IEnumerable<OrderLine> enumerable2 = (IEnumerable<OrderLine>)Enumerable.ToList<OrderLine>(Enumerable.Concat<OrderLine>(Enumerable.Where<OrderLine>((IEnumerable<OrderLine>)allOrderLines, (Func<OrderLine, bool>)(ol => Enumerable.Any<OrderLine>((IEnumerable<OrderLine>)orderToCompareAllOrderLines, (Func<OrderLine, bool>)(col =>
            {
                if (col.Id == ol.Id)
                    return !col.Equals((object)ol);
                else
                    return false;
            })))), enumerable1));
            if (Enumerable.Any<OrderLine>(enumerable2))
                NotificationCenter.Order.OnOrderLinesUpdated(this, enumerable2);
            IEnumerable<OrderLine> enumerable3 = (IEnumerable<OrderLine>)Enumerable.ToList<OrderLine>(Enumerable.Select<OrderLine, OrderLine>(Enumerable.Where<OrderLine>((IEnumerable<OrderLine>)orderToCompareAllOrderLines, (Func<OrderLine, bool>)(col => Enumerable.All<OrderLine>((IEnumerable<OrderLine>)allOrderLines, (Func<OrderLine, bool>)(ol => ol.Id != col.Id)))), (Func<OrderLine, OrderLine>)(col => col.Copy())));
            if (Enumerable.Any<OrderLine>(enumerable3))
                NotificationCenter.Order.OnOrderLinesRemoved(this, enumerable3);
            IEnumerable<CustomProperty> enumerable4 = (IEnumerable<CustomProperty>)Enumerable.ToList<CustomProperty>(Enumerable.Where<CustomProperty>((IEnumerable<CustomProperty>)this.Properties, (Func<CustomProperty, bool>)(p => Enumerable.All<CustomProperty>((IEnumerable<CustomProperty>)orderToCompare.Properties, (Func<CustomProperty, bool>)(cp => cp.Alias != p.Alias)))));
            if (Enumerable.Any<CustomProperty>(enumerable4))
                NotificationCenter.Order.OnOrderPropertiesAdded(this, enumerable4);
            IEnumerable<CustomProperty> enumerable5 = (IEnumerable<CustomProperty>)Enumerable.ToList<CustomProperty>(Enumerable.Concat<CustomProperty>(Enumerable.Where<CustomProperty>((IEnumerable<CustomProperty>)this.Properties, (Func<CustomProperty, bool>)(p => Enumerable.Any<CustomProperty>((IEnumerable<CustomProperty>)orderToCompare.Properties, (Func<CustomProperty, bool>)(cp =>
            {
                if (cp.Alias == p.Alias)
                    return !cp.Equals((object)p);
                else
                    return false;
            })))), enumerable4));
            if (Enumerable.Any<CustomProperty>(enumerable5))
                NotificationCenter.Order.OnOrderPropertiesUpdated(this, enumerable5);
            IEnumerable<CustomProperty> enumerable6 = (IEnumerable<CustomProperty>)Enumerable.ToList<CustomProperty>(Enumerable.Select<CustomProperty, CustomProperty>(Enumerable.Where<CustomProperty>((IEnumerable<CustomProperty>)orderToCompare.Properties, (Func<CustomProperty, bool>)(cp => Enumerable.All<CustomProperty>((IEnumerable<CustomProperty>)this.Properties, (Func<CustomProperty, bool>)(p => p.Alias != cp.Alias)))), (Func<CustomProperty, CustomProperty>)(cp => cp.Copy())));
            if (Enumerable.Any<CustomProperty>(enumerable6))
                NotificationCenter.Order.OnOrderPropertiesRemoved(this, enumerable6);
            if (this.PaymentInformation.CountryId != orderToCompare.PaymentInformation.CountryId)
                NotificationCenter.Order.OnPaymentCountryChanged(this);
            long? countryRegionId1 = this.PaymentInformation.CountryRegionId;
            long? countryRegionId2 = orderToCompare.PaymentInformation.CountryRegionId;
            if ((countryRegionId1.GetValueOrDefault() != countryRegionId2.GetValueOrDefault() ? 1 : (countryRegionId1.HasValue != countryRegionId2.HasValue ? 1 : 0)) != 0)
                NotificationCenter.Order.OnPaymentCountryRegionChanged(this);
            long? paymentMethodId1 = this.PaymentInformation.PaymentMethodId;
            long? paymentMethodId2 = orderToCompare.PaymentInformation.PaymentMethodId;
            if ((paymentMethodId1.GetValueOrDefault() != paymentMethodId2.GetValueOrDefault() ? 1 : (paymentMethodId1.HasValue != paymentMethodId2.HasValue ? 1 : 0)) != 0)
                NotificationCenter.Order.OnPaymentMethodChanged(this);
            long? countryId1 = this.ShipmentInformation.CountryId;
            long? countryId2 = orderToCompare.ShipmentInformation.CountryId;
            if ((countryId1.GetValueOrDefault() != countryId2.GetValueOrDefault() ? 1 : (countryId1.HasValue != countryId2.HasValue ? 1 : 0)) != 0)
                NotificationCenter.Order.OnShippingCountryChanged(this);
            long? countryRegionId3 = this.ShipmentInformation.CountryRegionId;
            long? countryRegionId4 = orderToCompare.ShipmentInformation.CountryRegionId;
            if ((countryRegionId3.GetValueOrDefault() != countryRegionId4.GetValueOrDefault() ? 1 : (countryRegionId3.HasValue != countryRegionId4.HasValue ? 1 : 0)) != 0)
                NotificationCenter.Order.OnShippingCountryRegionChanged(this);
            long? shippingMethodId1 = this.ShipmentInformation.ShippingMethodId;
            long? shippingMethodId2 = orderToCompare.ShipmentInformation.ShippingMethodId;
            if ((shippingMethodId1.GetValueOrDefault() != shippingMethodId2.GetValueOrDefault() ? 1 : (shippingMethodId1.HasValue != shippingMethodId2.HasValue ? 1 : 0)) != 0)
                NotificationCenter.Order.OnShippingMethodChanged(this);
            PaymentState? paymentState1 = this.TransactionInformation.PaymentState;
            PaymentState? paymentState2 = orderToCompare.TransactionInformation.PaymentState;
            if ((paymentState1.GetValueOrDefault() != paymentState2.GetValueOrDefault() ? 1 : (paymentState1.HasValue != paymentState2.HasValue ? 1 : 0)) != 0)
                NotificationCenter.Order.OnPaymentStateChanged(this);
            if (this.OrderStatusId != orderToCompare.OrderStatusId)
                NotificationCenter.Order.OnOrderStatusChanged(this);
            if (!this.IsFinalized || orderToCompare.IsFinalized)
                return;
            NotificationCenter.Order.OnFinalized(this);
        }

        public Order Copy()
        {
            return new Order()
            {
                StoreId = this.StoreId,
                CopiedFromOrderId = new Guid?(this.Id),
                CurrencyId = this.CurrencyId,
                OrderStatusId = this.OrderStatusId,
                VatGroupId = this.VatGroupId,
                LanguageId = this.LanguageId,
                CartNumber = this.CartNumber,
                OrderNumber = this.OrderNumber,
                CustomerId = this.CustomerId,
                DateCreated = this.DateCreated,
                DateModified = this.DateModified,
                DateFinalized = this.DateFinalized,
                OrderLines = this.OrderLines.Copy(),
                Properties = this.Properties.Copy(),
                PaymentInformation = this.PaymentInformation.Copy(),
                ShipmentInformation = this.ShipmentInformation.Copy(),
                TransactionInformation = this.TransactionInformation.Copy(),
                VatRate = this.VatRate,
                SubtotalPrices = this.SubtotalPrices.Copy(),
                SubtotalPrice = this.SubtotalPrice.Copy(),
                TotalPrices = this.TotalPrices.Copy(),
                TotalPrice = this.TotalPrice.Copy()
            };
        }

        public override bool Equals(object obj)
        {
            Order order = obj as Order;
            if (order == null || !(this.Id == order.Id) || (this.StoreId != order.StoreId || this.CurrencyId != order.CurrencyId) || (this.OrderStatusId != order.OrderStatusId || this.VatGroupId != order.VatGroupId))
                return false;
            long? languageId1 = this.LanguageId;
            long? languageId2 = order.LanguageId;
            if ((languageId1.GetValueOrDefault() != languageId2.GetValueOrDefault() ? 0 : (languageId1.HasValue == languageId2.HasValue ? 1 : 0)) != 0 && this.CartNumber == order.CartNumber && (this.OrderNumber == order.OrderNumber && this.CustomerId == order.CustomerId) && this.DateCreated.AddTicks(-(this.DateCreated.Ticks % 10000000L)) == order.DateCreated.AddTicks(-(order.DateCreated.Ticks % 10000000L)) && this.DateModified.AddTicks(-(this.DateModified.Ticks % 10000000L)) == order.DateModified.AddTicks(-(order.DateModified.Ticks % 10000000L)) && (!this.DateFinalized.HasValue && !order.DateFinalized.HasValue || this.DateFinalized.HasValue && order.DateFinalized.HasValue && this.DateFinalized.Value.AddTicks(-(this.DateFinalized.Value.Ticks % 10000000L)) == order.DateFinalized.Value.AddTicks(-(order.DateFinalized.Value.Ticks % 10000000L))) && (this.OrderLines.Equals((object)order.OrderLines) && this.Properties.Equals((object)order.Properties) && (this.PaymentInformation.Equals((object)order.PaymentInformation) && this.ShipmentInformation.Equals((object)order.ShipmentInformation)) && (this.TransactionInformation.Equals((object)order.TransactionInformation) && this.VatRate.Equals((object)order.VatRate) && (this.SubtotalPrices.Equals((object)order.SubtotalPrices) && this.SubtotalPrice.Equals((object)order.SubtotalPrice))) && this.TotalPrices.Equals((object)order.TotalPrices)))
                return this.TotalPrice.Equals((object)order.TotalPrice);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}