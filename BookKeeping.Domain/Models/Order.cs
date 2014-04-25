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
        public Guid Id
        {
            get;
            set;
        }

        public long StoreId
        {
            get;
            set;
        }

        public Guid? CopiedFromOrderId
        {
            get;
            set;
        }

        public long CurrencyId
        {
            get;
            set;
        }

        public long OrderStatusId
        {
            get;
            set;
        }

        public long VatGroupId
        {
            get;
            set;
        }

        public long? LanguageId
        {
            get;
            set;
        }

        public string CartNumber
        {
            get;
            set;
        }

        public string OrderNumber
        {
            get;
            set;
        }

        public string CustomerId
        {
            get;
            set;
        }

        public DateTime DateCreated
        {
            get;
            set;
        }

        public DateTime DateModified
        {
            get;
            set;
        }

        public DateTime? DateFinalized
        {
            get;
            set;
        }

        public bool IsFinalized
        {
            get
            {
                return this.DateFinalized.HasValue;
            }
        }

        public decimal TotalQuantity
        {
            get
            {
                return this.OrderLines.Sum((OrderLine ol) => ol.Quantity);
            }
        }

        public OrderLineCollection OrderLines
        {
            get;
            set;
        }

        public CustomPropertyCollection Properties
        {
            get;
            set;
        }

        public PaymentInformation PaymentInformation
        {
            get;
            set;
        }

        public ShipmentInformation ShipmentInformation
        {
            get;
            set;
        }

        public TransactionInformation TransactionInformation
        {
            get;
            set;
        }

        public VatRate VatRate
        {
            get;
            set;
        }

        public PriceCollection SubtotalPrices
        {
            get;
            set;
        }

        public Price SubtotalPrice
        {
            get;
            set;
        }

        public PriceCollection TotalPrices
        {
            get;
            set;
        }

        public Price TotalPrice
        {
            get;
            set;
        }

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
                this.CartNumber = store.CartNumberPrefix + store.GetNextOrderNumber(false);
            }
            this.AutoSetCustomerInformation(store);
            this.RemoveEmptyOrderLines(this.OrderLines);
            DependencyResolver.Current.GetService<IOrderCalculator>().CalculateOrder(this);
            IOrderRepository orderRepository = DependencyResolver.Current.GetService<IOrderRepository>();
            Order order = orderRepository.Get(this.StoreId, this.Id) ?? new Order();
            if (!this.Equals(order))
            {
                this.DateModified = DateTime.Now;
                this.CompareAndNotifyBeforeSave(order);
                this.AutoSetCustomerInformation(store);
                this.RemoveEmptyOrderLines(this.OrderLines);
                orderRepository.Save(this);
                this.CompareAndNotifyAfterSave(order);
            }
        }

        private void AutoSetCustomerInformation(Store store)
        {
            CustomProperty customProperty = this.Properties.Get(store.FirstNamePropertyAlias);
            if (customProperty != null)
            {
                this.PaymentInformation.FirstName = customProperty.Value;
            }
            customProperty = this.Properties.Get(store.LastNamePropertyAlias);
            if (customProperty != null)
            {
                this.PaymentInformation.LastName = customProperty.Value;
            }
            customProperty = this.Properties.Get(store.EmailPropertyAlias);
            if (customProperty != null)
            {
                this.PaymentInformation.Email = customProperty.Value;
            }
        }

        private void RemoveEmptyOrderLines(List<OrderLine> orderLines)
        {
            orderLines.RemoveAll((OrderLine ol) => ol.Quantity <= 0m);
            foreach (OrderLine current in orderLines)
            {
                this.RemoveEmptyOrderLines(current.OrderLines);
            }
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
            {
                this.CurrencyId = currencyId;
            }
            return flag;
        }

        public bool ChangePaymentRegion(long countryId, long? countryRegionId = null)
        {
            Country country = CountryService.Instance.Get(this.StoreId, countryId);
            CountryRegion countryRegion = countryRegionId.HasValue ? CountryRegionService.Instance.Get(this.StoreId, countryRegionId.Value) : null;
            bool flag = country != null && !country.IsDeleted && (!countryRegionId.HasValue || (countryRegion != null && !countryRegion.IsDeleted));
            if (flag)
            {
                this.PaymentInformation.CountryId = countryId;
                this.PaymentInformation.CountryRegionId = countryRegionId;
                Currency currency = CurrencyService.Instance.Get(this.StoreId, this.CurrencyId);
                if (currency == null || !currency.IsAllowedInCountry(this.PaymentInformation.CountryId))
                {
                    this.CurrencyId = country.DefaultCurrencyId;
                }
                if (this.PaymentInformation.PaymentMethodId.HasValue)
                {
                    PaymentMethod paymentMethod = PaymentMethodService.Instance.Get(this.StoreId, this.PaymentInformation.PaymentMethodId.Value);
                    if (paymentMethod == null || !paymentMethod.IsAllowedInRegion(this.PaymentInformation.CountryId, this.PaymentInformation.CountryRegionId))
                    {
                        this.PaymentInformation.PaymentMethodId = ((this.PaymentInformation.CountryRegionId.HasValue && countryRegion != null) ? countryRegion.DefaultPaymentMethodId : country.DefaultPaymentMethodId);
                    }
                }
                if (this.ShipmentInformation.ShippingMethodId.HasValue)
                {
                    ShippingMethod shippingMethod = ShippingMethodService.Instance.Get(this.StoreId, this.ShipmentInformation.ShippingMethodId.Value);
                    if (shippingMethod == null || !shippingMethod.IsAllowedInRegion(this.ShipmentInformation.CountryId ?? this.PaymentInformation.CountryId, this.ShipmentInformation.CountryId.HasValue ? this.ShipmentInformation.CountryRegionId : this.PaymentInformation.CountryRegionId))
                    {
                        this.ShipmentInformation.ShippingMethodId = null;
                    }
                }
            }
            return flag;
        }

        public bool ChangeShippingRegion(long? countryId, long? countryRegionId = null)
        {
            Country country = countryId.HasValue ? CountryService.Instance.Get(this.StoreId, countryId.Value) : null;
            CountryRegion countryRegion = countryRegionId.HasValue ? CountryRegionService.Instance.Get(this.StoreId, countryRegionId.Value) : null;
            bool flag = (!countryId.HasValue && !countryRegionId.HasValue) || (country != null && !country.IsDeleted && (!countryRegionId.HasValue || (countryRegion != null && !countryRegion.IsDeleted)));
            if (flag)
            {
                this.ShipmentInformation.CountryId = countryId;
                this.ShipmentInformation.CountryRegionId = countryRegionId;
                if (this.ShipmentInformation.ShippingMethodId.HasValue)
                {
                    ShippingMethod shippingMethod = ShippingMethodService.Instance.Get(this.StoreId, this.ShipmentInformation.ShippingMethodId.Value);
                    if (shippingMethod == null || !shippingMethod.IsAllowedInRegion(this.ShipmentInformation.CountryId ?? this.PaymentInformation.CountryId, this.ShipmentInformation.CountryId.HasValue ? this.ShipmentInformation.CountryRegionId : this.PaymentInformation.CountryRegionId))
                    {
                        this.ShipmentInformation.ShippingMethodId = null;
                    }
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
                flag = (paymentMethod != null && paymentMethod.IsAllowedInRegion(this.PaymentInformation.CountryId, this.PaymentInformation.CountryRegionId));
            }
            if (flag)
            {
                this.PaymentInformation.PaymentMethodId = paymentMethodId;
            }
            return flag;
        }

        public bool ChangeShippingMethod(long? shippingMethodId)
        {
            bool flag = !shippingMethodId.HasValue;
            if (shippingMethodId.HasValue)
            {
                ShippingMethod shippingMethod = ShippingMethodService.Instance.Get(this.StoreId, shippingMethodId.Value);
                flag = (shippingMethod != null && shippingMethod.IsAllowedInRegion(this.ShipmentInformation.CountryId ?? this.PaymentInformation.CountryId, this.ShipmentInformation.CountryId.HasValue ? this.ShipmentInformation.CountryRegionId : this.PaymentInformation.CountryRegionId));
            }
            if (flag)
            {
                this.ShipmentInformation.ShippingMethodId = shippingMethodId;
            }
            return flag;
        }

        public void Finalize(decimal amountAuthorized, string transactionId, PaymentState paymentState, string paymentType = null, string paymentIdentifier = null)
        {
            if (!this.IsFinalized)
            {
                Store store = StoreService.Instance.Get(this.StoreId);
                this.DateFinalized = new DateTime?(DateTime.Now);
                this.OrderNumber = store.OrderNumberPrefix + store.GetNextOrderNumber(true);
                Currency currency = CurrencyService.Instance.Get(this.StoreId, this.CurrencyId);
                this.TransactionInformation.AmountAuthorized = new PriceWithoutVat(amountAuthorized, currency);
                this.TransactionInformation.TransactionFee = new PriceWithoutVat(amountAuthorized - this.TotalPrice.WithVat, currency);
                this.TransactionInformation.TransactionId = transactionId;
                this.TransactionInformation.PaymentState = new PaymentState?(paymentState);
                this.TransactionInformation.PaymentType = paymentType;
                this.TransactionInformation.PaymentIdentifier = paymentIdentifier;
                this.Save();
                this.RemoveItemsFromStock(this.OrderLines, 1m);
            }
        }

        private void RemoveItemsFromStock(IEnumerable<OrderLine> orderLines, decimal parentOrderLineQuantity)
        {
            foreach (OrderLine current in orderLines)
            {
                this.RemoveItemsFromStock(current.OrderLines, current.Quantity * parentOrderLineQuantity);
                decimal? stock = ProductService.Instance.GetStock(this.StoreId, current.Sku);
                if (stock.HasValue)
                {
                    ProductService.Instance.SetStock(this.StoreId, current.Sku, new decimal?(stock.Value - current.Quantity * parentOrderLineQuantity));
                }
            }
        }

        private void CompareAndNotifyBeforeSave(Order orderToCompare)
        {
            Contract.Requires<ArgumentNullException>(orderToCompare != null, "orderToCompare");
            if (orderToCompare.Id == Guid.Empty)
            {
                NotificationCenter.Order.OnCreating(this);
            }
            NotificationCenter.Order.OnUpdating(this);
            if (this.LanguageId != orderToCompare.LanguageId)
            {
                NotificationCenter.Order.OnLanguageChanging(this);
            }
            if (this.CustomerId != orderToCompare.CustomerId)
            {
                NotificationCenter.Order.OnCustomerChanging(this);
            }
            if (this.VatGroupId != orderToCompare.VatGroupId)
            {
                NotificationCenter.Order.OnVatGroupChanging(this);
            }
            if (this.CurrencyId != orderToCompare.CurrencyId)
            {
                NotificationCenter.Order.OnCurrencyChanging(this);
            }
            List<OrderLine> allOrderLines = this.OrderLines.GetAll().ToList<OrderLine>();
            List<OrderLine> orderToCompareAllOrderLines = orderToCompare.OrderLines.GetAll().ToList<OrderLine>();
            IEnumerable<OrderLine> enumerable = (
                from ol in allOrderLines
                where orderToCompareAllOrderLines.All((OrderLine col) => col.Id != ol.Id)
                select ol).ToList<OrderLine>();
            if (enumerable.Any<OrderLine>())
            {
                NotificationCenter.Order.OnOrderLinesAdding(this, enumerable);
            }
            IEnumerable<OrderLine> enumerable2 = (
                from ol in allOrderLines
                where orderToCompareAllOrderLines.Any((OrderLine col) => col.Id == ol.Id && !col.Equals(ol))
                select ol).Concat(enumerable).ToList<OrderLine>();
            if (enumerable2.Any<OrderLine>())
            {
                NotificationCenter.Order.OnOrderLinesUpdating(this, enumerable2);
            }
            IList<OrderLine> list = new List<OrderLine>();
            foreach (OrderLine current in
                from col in orderToCompareAllOrderLines
                where allOrderLines.All((OrderLine ol) => ol.Id != col.Id)
                select col)
            {
                OrderLine orderLine = current.Copy();
                orderLine.Id = current.Id;
                list.Add(orderLine);
            }
            if (list.Any<OrderLine>())
            {
                NotificationCenter.Order.OnOrderLinesRemoving(this, list);
            }
            IEnumerable<CustomProperty> enumerable3 = (
                from p in this.Properties
                where orderToCompare.Properties.All((CustomProperty cp) => cp.Alias != p.Alias)
                select p).ToList<CustomProperty>();
            if (enumerable3.Any<CustomProperty>())
            {
                NotificationCenter.Order.OnOrderPropertiesAdding(this, enumerable3);
            }
            IEnumerable<CustomProperty> enumerable4 = (
                from p in this.Properties
                where orderToCompare.Properties.Any((CustomProperty cp) => cp.Alias == p.Alias && !cp.Equals(p))
                select p).Concat(enumerable3).ToList<CustomProperty>();
            if (enumerable4.Any<CustomProperty>())
            {
                NotificationCenter.Order.OnOrderPropertiesUpdating(this, enumerable4);
            }
            IEnumerable<CustomProperty> enumerable5 = (
                from cp in orderToCompare.Properties
                where this.Properties.All((CustomProperty p) => p.Alias != cp.Alias)
                select cp.Copy()).ToList<CustomProperty>();
            if (enumerable5.Any<CustomProperty>())
            {
                NotificationCenter.Order.OnOrderPropertiesRemoving(this, enumerable5);
            }
            if (this.PaymentInformation.CountryId != orderToCompare.PaymentInformation.CountryId)
            {
                NotificationCenter.Order.OnPaymentCountryChanging(this);
            }
            if (this.PaymentInformation.CountryRegionId != orderToCompare.PaymentInformation.CountryRegionId)
            {
                NotificationCenter.Order.OnPaymentCountryRegionChanging(this);
            }
            if (this.PaymentInformation.PaymentMethodId != orderToCompare.PaymentInformation.PaymentMethodId)
            {
                NotificationCenter.Order.OnPaymentMethodChanging(this);
            }
            if (this.ShipmentInformation.CountryId != orderToCompare.ShipmentInformation.CountryId)
            {
                NotificationCenter.Order.OnShippingCountryChanging(this);
            }
            if (this.ShipmentInformation.CountryRegionId != orderToCompare.ShipmentInformation.CountryRegionId)
            {
                NotificationCenter.Order.OnShippingCountryRegionChanging(this);
            }
            if (this.ShipmentInformation.ShippingMethodId != orderToCompare.ShipmentInformation.ShippingMethodId)
            {
                NotificationCenter.Order.OnShippingMethodChanging(this);
            }
            if (this.TransactionInformation.PaymentState != orderToCompare.TransactionInformation.PaymentState)
            {
                NotificationCenter.Order.OnPaymentStateChanging(this);
            }
            if (this.OrderStatusId != orderToCompare.OrderStatusId)
            {
                NotificationCenter.Order.OnOrderStatusChanging(this);
            }
            if (this.IsFinalized && !orderToCompare.IsFinalized)
            {
                NotificationCenter.Order.OnFinalizing(this);
            }
        }

        private void CompareAndNotifyAfterSave(Order orderToCompare)
        {
            Contract.Requires<ArgumentNullException>(orderToCompare != null, "orderToCompare");
            if (orderToCompare.Id == Guid.Empty)
            {
                NotificationCenter.Order.OnCreated(this);
            }
            NotificationCenter.Order.OnUpdated(this);
            if (this.LanguageId != orderToCompare.LanguageId)
            {
                NotificationCenter.Order.OnLanguageChanged(this);
            }
            if (this.CustomerId != orderToCompare.CustomerId)
            {
                NotificationCenter.Order.OnCustomerChanged(this);
            }
            if (this.VatGroupId != orderToCompare.VatGroupId)
            {
                NotificationCenter.Order.OnVatGroupChanged(this);
            }
            if (this.CurrencyId != orderToCompare.CurrencyId)
            {
                NotificationCenter.Order.OnCurrencyChanged(this);
            }
            List<OrderLine> allOrderLines = this.OrderLines.GetAll().ToList<OrderLine>();
            List<OrderLine> orderToCompareAllOrderLines = orderToCompare.OrderLines.GetAll().ToList<OrderLine>();
            IEnumerable<OrderLine> enumerable = (
                from ol in allOrderLines
                where orderToCompareAllOrderLines.All((OrderLine col) => col.Id != ol.Id)
                select ol).ToList<OrderLine>();
            if (enumerable.Any<OrderLine>())
            {
                NotificationCenter.Order.OnOrderLineAdded(this, enumerable);
            }
            IEnumerable<OrderLine> enumerable2 = (
                from ol in allOrderLines
                where orderToCompareAllOrderLines.Any((OrderLine col) => col.Id == ol.Id && !col.Equals(ol))
                select ol).Concat(enumerable).ToList<OrderLine>();
            if (enumerable2.Any<OrderLine>())
            {
                NotificationCenter.Order.OnOrderLinesUpdated(this, enumerable2);
            }
            IEnumerable<OrderLine> enumerable3 = (
                from col in orderToCompareAllOrderLines
                where allOrderLines.All((OrderLine ol) => ol.Id != col.Id)
                select col.Copy()).ToList<OrderLine>();
            if (enumerable3.Any<OrderLine>())
            {
                NotificationCenter.Order.OnOrderLinesRemoved(this, enumerable3);
            }
            IEnumerable<CustomProperty> enumerable4 = (
                from p in this.Properties
                where orderToCompare.Properties.All((CustomProperty cp) => cp.Alias != p.Alias)
                select p).ToList<CustomProperty>();
            if (enumerable4.Any<CustomProperty>())
            {
                NotificationCenter.Order.OnOrderPropertiesAdded(this, enumerable4);
            }
            IEnumerable<CustomProperty> enumerable5 = (
                from p in this.Properties
                where orderToCompare.Properties.Any((CustomProperty cp) => cp.Alias == p.Alias && !cp.Equals(p))
                select p).Concat(enumerable4).ToList<CustomProperty>();
            if (enumerable5.Any<CustomProperty>())
            {
                NotificationCenter.Order.OnOrderPropertiesUpdated(this, enumerable5);
            }
            IEnumerable<CustomProperty> enumerable6 = (
                from cp in orderToCompare.Properties
                where this.Properties.All((CustomProperty p) => p.Alias != cp.Alias)
                select cp.Copy()).ToList<CustomProperty>();
            if (enumerable6.Any<CustomProperty>())
            {
                NotificationCenter.Order.OnOrderPropertiesRemoved(this, enumerable6);
            }
            if (this.PaymentInformation.CountryId != orderToCompare.PaymentInformation.CountryId)
            {
                NotificationCenter.Order.OnPaymentCountryChanged(this);
            }
            if (this.PaymentInformation.CountryRegionId != orderToCompare.PaymentInformation.CountryRegionId)
            {
                NotificationCenter.Order.OnPaymentCountryRegionChanged(this);
            }
            if (this.PaymentInformation.PaymentMethodId != orderToCompare.PaymentInformation.PaymentMethodId)
            {
                NotificationCenter.Order.OnPaymentMethodChanged(this);
            }
            if (this.ShipmentInformation.CountryId != orderToCompare.ShipmentInformation.CountryId)
            {
                NotificationCenter.Order.OnShippingCountryChanged(this);
            }
            if (this.ShipmentInformation.CountryRegionId != orderToCompare.ShipmentInformation.CountryRegionId)
            {
                NotificationCenter.Order.OnShippingCountryRegionChanged(this);
            }
            if (this.ShipmentInformation.ShippingMethodId != orderToCompare.ShipmentInformation.ShippingMethodId)
            {
                NotificationCenter.Order.OnShippingMethodChanged(this);
            }
            if (this.TransactionInformation.PaymentState != orderToCompare.TransactionInformation.PaymentState)
            {
                NotificationCenter.Order.OnPaymentStateChanged(this);
            }
            if (this.OrderStatusId != orderToCompare.OrderStatusId)
            {
                NotificationCenter.Order.OnOrderStatusChanged(this);
            }
            if (this.IsFinalized && !orderToCompare.IsFinalized)
            {
                NotificationCenter.Order.OnFinalized(this);
            }
        }

        public Order Copy()
        {
            return new Order
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
            return order != null && (this.Id == order.Id && this.StoreId == order.StoreId && this.CurrencyId == order.CurrencyId && this.OrderStatusId == order.OrderStatusId && this.VatGroupId == order.VatGroupId && this.LanguageId == order.LanguageId && this.CartNumber == order.CartNumber && this.OrderNumber == order.OrderNumber && this.CustomerId == order.CustomerId && this.DateCreated.AddTicks(-(this.DateCreated.Ticks % 10000000L)) == order.DateCreated.AddTicks(-(order.DateCreated.Ticks % 10000000L)) && this.DateModified.AddTicks(-(this.DateModified.Ticks % 10000000L)) == order.DateModified.AddTicks(-(order.DateModified.Ticks % 10000000L)) && ((!this.DateFinalized.HasValue && !order.DateFinalized.HasValue) || (this.DateFinalized.HasValue && order.DateFinalized.HasValue && this.DateFinalized.Value.AddTicks(-(this.DateFinalized.Value.Ticks % 10000000L)) == order.DateFinalized.Value.AddTicks(-(order.DateFinalized.Value.Ticks % 10000000L)))) && this.OrderLines.Equals(order.OrderLines) && this.Properties.Equals(order.Properties) && this.PaymentInformation.Equals(order.PaymentInformation) && this.ShipmentInformation.Equals(order.ShipmentInformation) && this.TransactionInformation.Equals(order.TransactionInformation) && this.VatRate.Equals(order.VatRate) && this.SubtotalPrices.Equals(order.SubtotalPrices) && this.SubtotalPrice.Equals(order.SubtotalPrice) && this.TotalPrices.Equals(order.TotalPrices)) && this.TotalPrice.Equals(order.TotalPrice);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}