using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using BookKeeping.Domain.InformationExtractors;
using BookKeeping.Domain.Models;
using BookKeeping.Domain.Services;

namespace BookKeeping.Domain.PriceCalculators
{
    public class OrderCalculator : IOrderCalculator
    {
        protected readonly IVatGroupService VatGroupService;
        protected readonly ICurrencyService CurrencyService;
        protected readonly IShippingMethodService ShippingMethodService;
        protected readonly IPaymentMethodService PaymentMethodService;
        protected IShippingCalculator ShippingCalculator;
        protected IPaymentCalculator PaymentCalculator;
        protected IProductInformationExtractor ProductInformationExtractor;

        public OrderCalculator(IVatGroupService vatGroupService, ICurrencyService currencyService, IShippingMethodService shippingMethodService, IPaymentMethodService paymentMethodService, IShippingCalculator shippingCalculator, IPaymentCalculator paymentCalculator, IProductInformationExtractor productInformationExtractor)
        {
            this.VatGroupService = vatGroupService;
            this.CurrencyService = currencyService;
            this.ShippingMethodService = shippingMethodService;
            this.PaymentMethodService = paymentMethodService;
            this.ShippingCalculator = shippingCalculator;
            this.PaymentCalculator = paymentCalculator;
            this.ProductInformationExtractor = productInformationExtractor;
        }

        public virtual void CalculateOrder(Order order)
        {
            Contract.Requires<ArgumentNullException>(order != null, "order");
            if (!order.IsFinalized)
            {
                this.CalculateVatRate(order);
            }
            this.CalculateOrderLines(order);
            this.CalculateSubtotals(order);
            if (!order.IsFinalized)
            {
                this.CalculateShippingCosts(order);
                this.CalculatePaymentCosts(order);
            }
            this.CalculateTransactionCosts(order);
            this.CalculateTotals(order);
        }

        protected void CalculateVatRate(Order order)
        {
            Contract.Requires<ArgumentNullException>(order != null, "order");
            order.VatRate = this.CalculateOrderVatRate(order);
        }

        protected virtual VatRate CalculateOrderVatRate(Order order)
        {
            Contract.Requires<ArgumentNullException>(order != null, "order");
            return this.VatGroupService.Get(order.StoreId, order.VatGroupId).GetVatRate(order.PaymentInformation.CountryId, order.PaymentInformation.CountryRegionId);
        }

        protected void CalculateOrderLines(Order order)
        {
            Contract.Requires<ArgumentNullException>(order != null, "order");
            foreach (OrderLine current in order.OrderLines)
            {
                this.CalculateOrderLine(order, current, order.VatRate);
            }
        }

        protected void CalculateOrderLine(Order order, OrderLine orderLine, VatRate fallbackVatRate)
        {
            Contract.Requires<ArgumentNullException>(order != null, "order");
            Contract.Requires<ArgumentNullException>(orderLine != null, "orderLine");
            Contract.Requires<ArgumentNullException>(fallbackVatRate != null, "fallbackVatRate");
            if (!order.IsFinalized)
            {
                orderLine.VatRate = this.CalculateOrderLineVatRate(orderLine, fallbackVatRate, order);
            }
            foreach (OrderLine current in orderLine.OrderLines)
            {
                this.CalculateOrderLine(order, current, orderLine.VatRate);
            }
            orderLine.UnitPrices = new PriceCollection();
            foreach (OriginalUnitPrice current2 in orderLine.OriginalUnitPrices)
            {
                Currency currency5 = this.CurrencyService.Get(order.StoreId, current2.CurrencyId);
                orderLine.UnitPrices.Add(new Price(this.CalculateOrderLineUnitPrice(current2, orderLine, currency5, order), orderLine.VatRate, currency5));
            }
            orderLine.UnitPrice = orderLine.UnitPrices.Get(order.CurrencyId);
            if (orderLine.UnitPrice == null)
            {
                Currency currency2 = this.CurrencyService.Get(order.StoreId, order.CurrencyId);
                OriginalUnitPrice originalUnitPrice = this.ProductInformationExtractor.GetSnapshot(orderLine.ProductIdentifier).OriginalUnitPrices.SingleOrDefault((OriginalUnitPrice p) => p.CurrencyId == order.CurrencyId) ?? new OriginalUnitPrice(0m, order.CurrencyId);
                orderLine.OriginalUnitPrices.Add(originalUnitPrice);
                orderLine.UnitPrices.Add(new Price(this.CalculateOrderLineUnitPrice(originalUnitPrice, orderLine, currency2, order), orderLine.VatRate, currency2));
                orderLine.UnitPrice = orderLine.UnitPrices.Get(order.CurrencyId);
            }
            orderLine.TotalPrices = new PriceCollection();
            foreach (Price current3 in orderLine.UnitPrices)
            {
                Currency currency3 = this.CurrencyService.Get(order.StoreId, current3.CurrencyId);
                orderLine.TotalPrices.Add(new Price(this.CalculateOrderLineTotalPrice(current3, orderLine, currency3, order), orderLine.VatRate, currency3));
            }
            orderLine.TotalPrice = orderLine.TotalPrices.Get(order.CurrencyId);
            orderLine.BundleUnitPrices = new PriceCollection();
            IEnumerable<Price> source = orderLine.OrderLines.SelectMany((OrderLine ol) => ol.BundleTotalPrices).ToList<Price>();
            foreach (Price current4 in orderLine.UnitPrices)
            {
                Currency currency = this.CurrencyService.Get(order.StoreId, current4.CurrencyId);
                orderLine.BundleUnitPrices.Add(this.CalculateOrderLineBundleUnitPrice(current4,
                    from p in source
                    where p.CurrencyId == currency.Id
                    select p, orderLine, currency, order));
            }
            orderLine.BundleUnitPrice = orderLine.BundleUnitPrices.Get(order.CurrencyId);
            orderLine.BundleTotalPrices = new PriceCollection();
            foreach (Price current5 in orderLine.BundleUnitPrices)
            {
                Currency currency4 = this.CurrencyService.Get(order.StoreId, current5.CurrencyId);
                orderLine.BundleTotalPrices.Add(this.CalculateOrderLineBundleTotalPrice(current5, orderLine, currency4, order));
            }
            orderLine.BundleTotalPrice = orderLine.BundleTotalPrices.Get(order.CurrencyId);
        }

        protected virtual VatRate CalculateOrderLineVatRate(OrderLine orderLine, VatRate fallbackVatRate, Order order)
        {
            Contract.Requires<ArgumentNullException>(order != null, "order");
            Contract.Requires<ArgumentNullException>(fallbackVatRate != null, "fallbackVatRate");
            Contract.Requires<ArgumentNullException>(orderLine != null, "orderLine");
            VatRate result = fallbackVatRate;
            if (orderLine.VatGroupId.HasValue)
            {
                result = this.VatGroupService.Get(order.StoreId, orderLine.VatGroupId.Value).GetVatRate(order.PaymentInformation.CountryId, order.PaymentInformation.CountryRegionId);
            }
            return result;
        }

        protected virtual decimal CalculateOrderLineUnitPrice(OriginalUnitPrice originalUnitPrice, OrderLine orderLine, Currency currency, Order order)
        {
            Contract.Requires<ArgumentNullException>(originalUnitPrice != null, "originalUnitPrice");
            Contract.Requires<ArgumentNullException>(orderLine != null, "orderLine");
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
            Contract.Requires<ArgumentNullException>(order != null, "order");
            return originalUnitPrice.Value;
        }

        protected virtual decimal CalculateOrderLineTotalPrice(Price unitPrice, OrderLine orderLine, Currency currency, Order order)
        {
            Contract.Requires<ArgumentNullException>(unitPrice != null, "unitPrice");
            Contract.Requires<ArgumentNullException>(orderLine != null, "orderLine");
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
            Contract.Requires<ArgumentNullException>(order != null, "order");
            return unitPrice.Value * orderLine.Quantity;
        }

        protected virtual Price CalculateOrderLineBundleUnitPrice(Price unitPrice, IEnumerable<Price> subOrderLinesBundleTotalPrices, OrderLine orderLine, Currency currency, Order order)
        {
            subOrderLinesBundleTotalPrices = subOrderLinesBundleTotalPrices.ToList<Price>();
            Contract.Requires<ArgumentNullException>(unitPrice != null, "unitPrice");
            Contract.Requires<ArgumentNullException>(orderLine != null, "orderLine");
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
            Contract.Requires<ArgumentNullException>(order != null, "order");
            subOrderLinesBundleTotalPrices = subOrderLinesBundleTotalPrices.ToList<Price>();
            return new Price(unitPrice.Value + subOrderLinesBundleTotalPrices.Sum((Price p) => p.Value), unitPrice.Vat + subOrderLinesBundleTotalPrices.Sum((Price p) => p.Vat), unitPrice.WithVat + subOrderLinesBundleTotalPrices.Sum((Price p) => p.WithVat), currency);
        }

        protected virtual Price CalculateOrderLineBundleTotalPrice(Price bundleUnitPrice, OrderLine orderLine, Currency currency, Order order)
        {
            Contract.Requires<ArgumentNullException>(bundleUnitPrice != null, "bundleUnitPrice");
            Contract.Requires<ArgumentNullException>(orderLine != null, "orderLine");
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
            Contract.Requires<ArgumentNullException>(order != null, "order");
            return new Price(bundleUnitPrice.Value * orderLine.Quantity, bundleUnitPrice.Vat * orderLine.Quantity, bundleUnitPrice.WithVat * orderLine.Quantity, currency);
        }

        protected void CalculateSubtotals(Order order)
        {
            Contract.Requires<ArgumentNullException>(order != null, "order");
            order.SubtotalPrices = new PriceCollection();
            if (order.OrderLines.Count > 0)
            {
                foreach (IGrouping<long, Price> grouping in order.OrderLines.SelectMany(ol => ol.BundleTotalPrices).GroupBy(p => p.CurrencyId))
                {
                    Currency currency = this.CurrencyService.Get(order.StoreId, grouping.Key);
                    order.SubtotalPrices.Add(this.CalculateOrderSubtotalPrice((IEnumerable<Price>)grouping, currency, order));
                }
            }
            else
            {
                foreach (Currency currency in this.CurrencyService.GetAll(order.StoreId))
                {
                    order.SubtotalPrices.Add(new Price(0m, order.VatRate, currency));
                }
            }
            order.SubtotalPrice = order.SubtotalPrices.Get(order.CurrencyId);
        }

        protected virtual Price CalculateOrderSubtotalPrice(IEnumerable<Price> orderLinesBundleTotalPrices, Currency currency, Order order)
        {
            orderLinesBundleTotalPrices = orderLinesBundleTotalPrices.ToList<Price>();
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
            Contract.Requires<ArgumentNullException>(order != null, "order");
            decimal num = orderLinesBundleTotalPrices.Sum((Price p) => p.Value);
            decimal num2 = orderLinesBundleTotalPrices.Sum((Price p) => p.Vat);
            return new Price(num, num2, num + num2, currency);
        }

        protected void CalculateShippingCosts(Order order)
        {
            Contract.Requires<ArgumentNullException>(order != null, "order");
            if (order.ShipmentInformation.ShippingMethodId.HasValue)
            {
                ShippingMethod shippingMethod = this.ShippingMethodService.Get(order.StoreId, order.ShipmentInformation.ShippingMethodId.Value);
                order.ShipmentInformation.VatRate = this.ShippingCalculator.CalculateVatRate(shippingMethod, order);
                order.ShipmentInformation.TotalPrices = new PriceCollection();
                order.ShipmentInformation.TotalPrices.AddRange(this.ShippingCalculator.CalculatePrices(shippingMethod, order));
            }
            else
            {
                order.ShipmentInformation.VatRate = order.VatRate;
                order.ShipmentInformation.TotalPrices = new PriceCollection();
                foreach (Currency current in this.CurrencyService.GetAll(order.StoreId))
                {
                    order.ShipmentInformation.TotalPrices.Add(new Price(0m, order.ShipmentInformation.VatRate, current));
                }
            }
            order.ShipmentInformation.TotalPrice = order.ShipmentInformation.TotalPrices.Get(order.CurrencyId);
        }

        protected void CalculatePaymentCosts(Order order)
        {
            Contract.Requires<ArgumentNullException>(order != null, "order");
            if (order.PaymentInformation.PaymentMethodId.HasValue)
            {
                PaymentMethod paymentMethod = this.PaymentMethodService.Get(order.StoreId, order.PaymentInformation.PaymentMethodId.Value);
                order.PaymentInformation.VatRate = this.PaymentCalculator.CalculateVatRate(paymentMethod, order);
                order.PaymentInformation.TotalPrices = new PriceCollection();
                order.PaymentInformation.TotalPrices.AddRange(this.PaymentCalculator.CalculatePrices(paymentMethod, order));
            }
            else
            {
                order.PaymentInformation.VatRate = order.VatRate;
                order.PaymentInformation.TotalPrices = new PriceCollection();
                foreach (Currency current in this.CurrencyService.GetAll(order.StoreId))
                {
                    order.PaymentInformation.TotalPrices.Add(new Price(0m, order.PaymentInformation.VatRate, current));
                }
            }
            order.PaymentInformation.TotalPrice = order.PaymentInformation.TotalPrices.Get(order.CurrencyId);
        }

        protected void CalculateTransactionCosts(Order order)
        {
            Contract.Requires<ArgumentNullException>(order != null, "order");
            order.TransactionInformation.TransactionFee = new PriceWithoutVat(this.CalculateOrderTransactionFee(order), this.CurrencyService.Get(order.StoreId, order.CurrencyId));
        }

        protected virtual decimal CalculateOrderTransactionFee(Order order)
        {
            Contract.Requires<ArgumentNullException>(order != null, "order");
            return order.TransactionInformation.TransactionFee.Value;
        }

        protected void CalculateTotals(Order order)
        {
            Contract.Requires<ArgumentNullException>(order != null, "order");
            order.TotalPrices = new PriceCollection();
            foreach (Price current in order.SubtotalPrices)
            {
                Currency currency = this.CurrencyService.Get(order.StoreId, current.CurrencyId);
                Price shipmentTotalPrice = order.ShipmentInformation.TotalPrices.Get(current.CurrencyId);
                Price paymentTotalPrice = order.PaymentInformation.TotalPrices.Get(current.CurrencyId);
                PriceWithoutVat transactionFee = (order.TransactionInformation.TransactionFee.CurrencyId == current.CurrencyId) ? order.TransactionInformation.TransactionFee : new PriceWithoutVat(0m, currency);
                order.TotalPrices.Add(this.CalculateOrderTotalPrice(current, shipmentTotalPrice, paymentTotalPrice, transactionFee, currency, order));
            }
            order.TotalPrice = order.TotalPrices.Get(order.CurrencyId);
        }

        protected virtual Price CalculateOrderTotalPrice(Price subtotalPrice, Price shipmentTotalPrice, Price paymentTotalPrice, PriceWithoutVat transactionFee, Currency currency, Order order)
        {
            Contract.Requires<ArgumentNullException>(subtotalPrice != null, "subtotalPrice");
            Contract.Requires<ArgumentNullException>(shipmentTotalPrice != null, "shipmentTotalPrice");
            Contract.Requires<ArgumentNullException>(paymentTotalPrice != null, "paymentTotalPrice");
            Contract.Requires<ArgumentNullException>(transactionFee != null, "transactionFee");
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
            Contract.Requires<ArgumentNullException>(order != null, "order");
            decimal num = subtotalPrice.Value + shipmentTotalPrice.Value + paymentTotalPrice.Value + transactionFee.Value;
            decimal num2 = subtotalPrice.Vat + shipmentTotalPrice.Vat + paymentTotalPrice.Vat;
            return new Price(num, num2, num + num2, currency);
        }
    }
}