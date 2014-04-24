using System.Collections.Generic;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.Notifications
{
    public static class NotificationCenter
    {
        public static class Country
        {
            public static event CountryEventHandler Created = param0 => { };

            public static event CountryEventHandler Deleted = param0 => { };

            static Country()
            {
            }

            internal static void OnCreated(BookKeeping.Domain.Models.Country country)
            {
                NotificationCenter.Country.Created(country);
            }

            internal static void OnDeleted(BookKeeping.Domain.Models.Country country)
            {
                NotificationCenter.Country.Deleted(country);
            }
        }

        public static class CountryRegion
        {
            public static event CountryRegionEventHandler Created = param0 => { };

            public static event CountryRegionEventHandler Deleted = param0 => { };

            static CountryRegion()
            {
            }

            internal static void OnCreated(BookKeeping.Domain.Models.CountryRegion countryRegion)
            {
                NotificationCenter.CountryRegion.Created(countryRegion);
            }

            internal static void OnDeleted(BookKeeping.Domain.Models.CountryRegion countryRegion)
            {
                NotificationCenter.CountryRegion.Deleted(countryRegion);
            }
        }

        public static class Currency
        {
            public static event CurrencyEventHandler Created = param0 => { };

            public static event CurrencyEventHandler Deleted = param0 => { };

            static Currency()
            {
            }

            internal static void OnCreated(BookKeeping.Domain.Models.Currency currency)
            {
                NotificationCenter.Currency.Created(currency);
            }

            internal static void OnDeleted(BookKeeping.Domain.Models.Currency currency)
            {
                NotificationCenter.Currency.Deleted(currency);
            }
        }

        public static class Order
        {
            public static event OrderEventHandler Creating = param0 => { };

            public static event OrderEventHandler Created = param0 => { };

            public static event OrderEventHandler Updating = param0 => { };

            public static event OrderEventHandler Updated = param0 => { };

            public static event OrderEventHandler Deleting = param0 => { };

            public static event OrderEventHandler Deleted = param0 => { };

            public static event OrderEventHandler CurrencyChanging = param0 => { };

            public static event OrderEventHandler CurrencyChanged = param0 => { };

            public static event OrderEventHandler OrderStatusChanging = param0 => { };

            public static event OrderEventHandler OrderStatusChanged = param0 => { };

            public static event OrderEventHandler VatGroupChanging = param0 => { };

            public static event OrderEventHandler VatGroupChanged = param0 => { };

            public static event OrderEventHandler LanguageChanging = param0 => { };

            public static event OrderEventHandler LanguageChanged = param0 => { };

            public static event OrderEventHandler CustomerChanging = param0 => { };

            public static event OrderEventHandler CustomerChanged = param0 => { };

            public static event OrderLinesEventHandler OrderLinesAdding = (param0, param1) => { };

            public static event OrderLinesEventHandler OrderLinesAdded = (param0, param1) => { };

            public static event OrderLinesEventHandler OrderLinesUpdating = (param0, param1) => { };

            public static event OrderLinesEventHandler OrderLinesUpdated = (param0, param1) => { };

            public static event OrderLinesEventHandler OrderLinesRemoving = (param0, param1) => { };

            public static event OrderLinesEventHandler OrderLinesRemoved = (param0, param1) => { };

            public static event OrderPropertiesEventHandler OrderPropertiesAdding = (param0, param1) => { };

            public static event OrderPropertiesEventHandler OrderPropertiesAdded = (param0, param1) => { };

            public static event OrderPropertiesEventHandler OrderPropertiesUpdating = (param0, param1) => { };

            public static event OrderPropertiesEventHandler OrderPropertiesUpdated = (param0, param1) => { };

            public static event OrderPropertiesEventHandler OrderPropertiesRemoving = (param0, param1) => { };

            public static event OrderPropertiesEventHandler OrderPropertiesRemoved = (param0, param1) => { };

            public static event OrderEventHandler PaymentCountryChanging = param0 => { };

            public static event OrderEventHandler PaymentCountryChanged = param0 => { };

            public static event OrderEventHandler PaymentCountryRegionChanging = param0 => { };

            public static event OrderEventHandler PaymentCountryRegionChanged = param0 => { };

            public static event OrderEventHandler PaymentMethodChanging = param0 => { };

            public static event OrderEventHandler PaymentMethodChanged = param0 => { };

            public static event OrderEventHandler ShippingCountryChanging = param0 => { };

            public static event OrderEventHandler ShippingCountryChanged = param0 => { };

            public static event OrderEventHandler ShippingCountryRegionChanging = param0 => { };

            public static event OrderEventHandler ShippingCountryRegionChanged = param0 => { };

            public static event OrderEventHandler ShippingMethodChanging = param0 => { };

            public static event OrderEventHandler ShippingMethodChanged = param0 => { };

            public static event OrderEventHandler PaymentStateChanging = param0 => { };

            public static event OrderEventHandler PaymentStateChanged = param0 => { };

            public static event OrderEventHandler Finalizing = param0 => { };

            public static event OrderEventHandler Finalized = param0 => { };

            static Order()
            {
            }

            public static void OnCreating(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.Creating(order);
            }

            public static void OnCreated(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.Created(order);
            }

            public static void OnUpdating(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.Updating(order);
            }

            public static void OnUpdated(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.Updated(order);
            }

            public static void OnDeleting(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.Deleting(order);
            }

            public static void OnDeleted(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.Deleted(order);
            }

            public static void OnCurrencyChanging(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.CurrencyChanging(order);
            }

            public static void OnCurrencyChanged(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.CurrencyChanged(order);
            }

            public static void OnOrderStatusChanging(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.OrderStatusChanging(order);
            }

            public static void OnOrderStatusChanged(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.OrderStatusChanged(order);
            }

            public static void OnVatGroupChanging(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.VatGroupChanging(order);
            }

            public static void OnVatGroupChanged(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.VatGroupChanged(order);
            }

            public static void OnLanguageChanging(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.LanguageChanging(order);
            }

            public static void OnLanguageChanged(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.LanguageChanged(order);
            }

            public static void OnCustomerChanging(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.CustomerChanging(order);
            }

            public static void OnCustomerChanged(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.CustomerChanged(order);
            }

            public static void OnOrderLinesAdding(BookKeeping.Domain.Models.Order order, IEnumerable<OrderLine> orderLines)
            {
                NotificationCenter.Order.OrderLinesAdding(order, orderLines);
            }

            public static void OnOrderLineAdded(BookKeeping.Domain.Models.Order order, IEnumerable<OrderLine> orderLines)
            {
                NotificationCenter.Order.OrderLinesAdded(order, orderLines);
            }

            public static void OnOrderLinesUpdating(BookKeeping.Domain.Models.Order order, IEnumerable<OrderLine> orderLines)
            {
                NotificationCenter.Order.OrderLinesUpdating(order, orderLines);
            }

            public static void OnOrderLinesUpdated(BookKeeping.Domain.Models.Order order, IEnumerable<OrderLine> orderLines)
            {
                NotificationCenter.Order.OrderLinesUpdated(order, orderLines);
            }

            public static void OnOrderLinesRemoving(BookKeeping.Domain.Models.Order order, IEnumerable<OrderLine> orderLines)
            {
                NotificationCenter.Order.OrderLinesRemoving(order, orderLines);
            }

            public static void OnOrderLinesRemoved(BookKeeping.Domain.Models.Order order, IEnumerable<OrderLine> orderLines)
            {
                NotificationCenter.Order.OrderLinesRemoved(order, orderLines);
            }

            public static void OnOrderPropertiesAdding(BookKeeping.Domain.Models.Order order, IEnumerable<CustomProperty> properties)
            {
                NotificationCenter.Order.OrderPropertiesAdding(order, properties);
            }

            public static void OnOrderPropertiesAdded(BookKeeping.Domain.Models.Order order, IEnumerable<CustomProperty> properties)
            {
                NotificationCenter.Order.OrderPropertiesAdded(order, properties);
            }

            public static void OnOrderPropertiesUpdating(BookKeeping.Domain.Models.Order order, IEnumerable<CustomProperty> properties)
            {
                NotificationCenter.Order.OrderPropertiesUpdating(order, properties);
            }

            public static void OnOrderPropertiesUpdated(BookKeeping.Domain.Models.Order order, IEnumerable<CustomProperty> properties)
            {
                NotificationCenter.Order.OrderPropertiesUpdated(order, properties);
            }

            public static void OnOrderPropertiesRemoving(BookKeeping.Domain.Models.Order order, IEnumerable<CustomProperty> properties)
            {
                NotificationCenter.Order.OrderPropertiesRemoving(order, properties);
            }

            public static void OnOrderPropertiesRemoved(BookKeeping.Domain.Models.Order order, IEnumerable<CustomProperty> properties)
            {
                NotificationCenter.Order.OrderPropertiesRemoved(order, properties);
            }

            public static void OnPaymentCountryChanging(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.PaymentCountryChanging(order);
            }

            public static void OnPaymentCountryChanged(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.PaymentCountryChanged(order);
            }

            public static void OnPaymentCountryRegionChanging(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.PaymentCountryRegionChanging(order);
            }

            public static void OnPaymentCountryRegionChanged(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.PaymentCountryRegionChanged(order);
            }

            public static void OnPaymentMethodChanging(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.PaymentMethodChanging(order);
            }

            public static void OnPaymentMethodChanged(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.PaymentMethodChanged(order);
            }

            public static void OnShippingCountryChanging(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.ShippingCountryChanging(order);
            }

            public static void OnShippingCountryChanged(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.ShippingCountryChanged(order);
            }

            public static void OnShippingCountryRegionChanging(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.ShippingCountryRegionChanging(order);
            }

            public static void OnShippingCountryRegionChanged(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.ShippingCountryRegionChanged(order);
            }

            public static void OnShippingMethodChanging(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.ShippingMethodChanging(order);
            }

            public static void OnShippingMethodChanged(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.ShippingMethodChanged(order);
            }

            public static void OnPaymentStateChanging(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.PaymentStateChanging(order);
            }

            public static void OnPaymentStateChanged(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.PaymentStateChanged(order);
            }

            public static void OnFinalizing(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.Finalizing(order);
            }

            public static void OnFinalized(BookKeeping.Domain.Models.Order order)
            {
                NotificationCenter.Order.Finalized(order);
            }
        }

        public static class OrderStatus
        {
            public static event OrderStatusEventHandler Created = param0 => { };

            public static event OrderStatusEventHandler Deleted = param0 => { };

            static OrderStatus()
            {
            }

            internal static void OnCreated(BookKeeping.Domain.Models.OrderStatus orderStatus)
            {
                NotificationCenter.OrderStatus.Created(orderStatus);
            }

            internal static void OnDeleted(BookKeeping.Domain.Models.OrderStatus orderStatus)
            {
                NotificationCenter.OrderStatus.Deleted(orderStatus);
            }
        }

        public static class PaymentMethod
        {
            public static event PaymentMethodEventHandler Created = param0 => { };

            public static event PaymentMethodEventHandler Deleted = param0 => { };

            static PaymentMethod()
            {
            }

            internal static void OnCreated(BookKeeping.Domain.Models.PaymentMethod paymentMethod)
            {
                NotificationCenter.PaymentMethod.Created(paymentMethod);
            }

            internal static void OnDeleted(BookKeeping.Domain.Models.PaymentMethod paymentMethod)
            {
                NotificationCenter.PaymentMethod.Deleted(paymentMethod);
            }
        }

        public static class ShippingMethod
        {
            public static event ShippingMethodEventHandler Created = param0 => { };

            public static event ShippingMethodEventHandler Deleted = param0 => { };

            static ShippingMethod()
            {
            }

            internal static void OnCreated(BookKeeping.Domain.Models.ShippingMethod shippingMethod)
            {
                NotificationCenter.ShippingMethod.Created(shippingMethod);
            }

            internal static void OnDeleted(BookKeeping.Domain.Models.ShippingMethod shippingMethod)
            {
                NotificationCenter.ShippingMethod.Deleted(shippingMethod);
            }
        }

        public static class Store
        {
            public static event StoreEventHandler Created = param0 => { };

            public static event StoreEventHandler Deleted = param0 => { };

            static Store()
            {
            }

            internal static void OnCreated(BookKeeping.Domain.Models.Store store)
            {
                NotificationCenter.Store.Created(store);
            }

            internal static void OnDeleted(BookKeeping.Domain.Models.Store store)
            {
                NotificationCenter.Store.Deleted(store);
            }
        }

        public static class VatGroup
        {
            public static event VatGroupEventHandler Created = param0 => { };

            public static event VatGroupEventHandler Deleted = param0 => { };

            static VatGroup()
            {
            }

            internal static void OnCreated(BookKeeping.Domain.Models.VatGroup vatGroup)
            {
                NotificationCenter.VatGroup.Created(vatGroup);
            }

            internal static void OnDeleted(BookKeeping.Domain.Models.VatGroup vatGroup)
            {
                NotificationCenter.VatGroup.Deleted(vatGroup);
            }
        }
    }
}