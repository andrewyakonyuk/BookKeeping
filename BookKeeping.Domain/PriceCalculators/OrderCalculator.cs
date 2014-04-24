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
                this.CalculateVatRate(order);
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
            foreach (OrderLine orderLine in (List<OrderLine>)order.OrderLines)
                this.CalculateOrderLine(order, orderLine, order.VatRate);
        }

        protected void CalculateOrderLine(Order order, OrderLine orderLine, VatRate fallbackVatRate)
        {
            Contract.Requires<ArgumentNullException>(order != null, "order");
            Contract.Requires<ArgumentNullException>(orderLine != null, "orderLine");
            Contract.Requires<ArgumentNullException>(fallbackVatRate != null, "fallbackVatRate");
            if (!order.IsFinalized)
                orderLine.VatRate = this.CalculateOrderLineVatRate(orderLine, fallbackVatRate, order);
            foreach (OrderLine orderLine1 in (List<OrderLine>)orderLine.OrderLines)
                this.CalculateOrderLine(order, orderLine1, orderLine.VatRate);
            orderLine.UnitPrices = new PriceCollection();
            foreach (OriginalUnitPrice originalUnitPrice in (List<OriginalUnitPrice>)orderLine.OriginalUnitPrices)
            {
                BookKeeping.Domain.Models.Currency currency = this.CurrencyService.Get(order.StoreId, originalUnitPrice.CurrencyId);
                orderLine.UnitPrices.Add(new Price(this.CalculateOrderLineUnitPrice(originalUnitPrice, orderLine, currency, order), orderLine.VatRate, currency));
            }
            orderLine.UnitPrice = orderLine.UnitPrices.Get(order.CurrencyId);
            if (orderLine.UnitPrice == null)
            {
                BookKeeping.Domain.Models.Currency currency = this.CurrencyService.Get(order.StoreId, order.CurrencyId);
                OriginalUnitPrice originalUnitPrice = Enumerable.SingleOrDefault<OriginalUnitPrice>((IEnumerable<OriginalUnitPrice>)this.ProductInformationExtractor.GetSnapshot(orderLine.ProductIdentifier).OriginalUnitPrices, (Func<OriginalUnitPrice, bool>)(p => p.CurrencyId == order.CurrencyId)) ?? new OriginalUnitPrice(new Decimal(0), order.CurrencyId);
                orderLine.OriginalUnitPrices.Add(originalUnitPrice);
                orderLine.UnitPrices.Add(new Price(this.CalculateOrderLineUnitPrice(originalUnitPrice, orderLine, currency, order), orderLine.VatRate, currency));
                orderLine.UnitPrice = orderLine.UnitPrices.Get(order.CurrencyId);
            }
            orderLine.TotalPrices = new PriceCollection();
            foreach (Price unitPrice in (List<Price>)orderLine.UnitPrices)
            {
                BookKeeping.Domain.Models.Currency currency = this.CurrencyService.Get(order.StoreId, unitPrice.CurrencyId);
                orderLine.TotalPrices.Add(new Price(this.CalculateOrderLineTotalPrice(unitPrice, orderLine, currency, order), orderLine.VatRate, currency));
            }
            orderLine.TotalPrice = orderLine.TotalPrices.Get(order.CurrencyId);
            orderLine.BundleUnitPrices = new PriceCollection();
            IEnumerable<Price> source = (IEnumerable<Price>)Enumerable.ToList<Price>(Enumerable.SelectMany<OrderLine, Price>((IEnumerable<OrderLine>)orderLine.OrderLines, (Func<OrderLine, IEnumerable<Price>>)(ol => (IEnumerable<Price>)ol.BundleTotalPrices)));
            foreach (Price unitPrice in (List<Price>)orderLine.UnitPrices)
            {
                BookKeeping.Domain.Models.Currency currency = this.CurrencyService.Get(order.StoreId, unitPrice.CurrencyId);
                orderLine.BundleUnitPrices.Add(this.CalculateOrderLineBundleUnitPrice(unitPrice, Enumerable.Where<Price>(source, (Func<Price, bool>)(p => p.CurrencyId == currency.Id)), orderLine, currency, order));
            }
            orderLine.BundleUnitPrice = orderLine.BundleUnitPrices.Get(order.CurrencyId);
            orderLine.BundleTotalPrices = new PriceCollection();
            foreach (Price bundleUnitPrice in (List<Price>)orderLine.BundleUnitPrices)
            {
                BookKeeping.Domain.Models.Currency currency = this.CurrencyService.Get(order.StoreId, bundleUnitPrice.CurrencyId);
                orderLine.BundleTotalPrices.Add(this.CalculateOrderLineBundleTotalPrice(bundleUnitPrice, orderLine, currency, order));
            }
            orderLine.BundleTotalPrice = orderLine.BundleTotalPrices.Get(order.CurrencyId);
        }

        protected virtual VatRate CalculateOrderLineVatRate(OrderLine orderLine, VatRate fallbackVatRate, Order order)
        {
            Contract.Requires<ArgumentNullException>(order != null, "order");
            Contract.Requires<ArgumentNullException>(orderLine != null, "orderLine");
            Contract.Requires<ArgumentNullException>(fallbackVatRate != null, "fallbackVatRate");
            VatRate vatRate = fallbackVatRate;
            if (orderLine.VatGroupId.HasValue)
                vatRate = this.VatGroupService.Get(order.StoreId, orderLine.VatGroupId.Value).GetVatRate(order.PaymentInformation.CountryId, order.PaymentInformation.CountryRegionId);
            return vatRate;
        }

        protected virtual Decimal CalculateOrderLineUnitPrice(OriginalUnitPrice originalUnitPrice, OrderLine orderLine, Currency currency, Order order)
        {
            Contract.Requires<ArgumentNullException>(originalUnitPrice != null, "originalUnitPrice");
            Contract.Requires<ArgumentNullException>(order != null, "order");
            Contract.Requires<ArgumentNullException>(orderLine != null, "orderLine");
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
            return originalUnitPrice.Value;
        }

        protected virtual Decimal CalculateOrderLineTotalPrice(Price unitPrice, OrderLine orderLine, Currency currency, Order order)
        {
            Contract.Requires<ArgumentNullException>(unitPrice != null, "unitPrice");
            Contract.Requires<ArgumentNullException>(order != null, "order");
            Contract.Requires<ArgumentNullException>(orderLine != null, "orderLine");
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
            return unitPrice.Value * orderLine.Quantity;
        }

        protected virtual Price CalculateOrderLineBundleUnitPrice(Price unitPrice, IEnumerable<Price> subOrderLinesBundleTotalPrices, OrderLine orderLine, BookKeeping.Domain.Models.Currency currency, Order order)
        {
            subOrderLinesBundleTotalPrices = (IEnumerable<Price>)Enumerable.ToList<Price>(subOrderLinesBundleTotalPrices);
            Contract.Requires<ArgumentNullException>(unitPrice != null, "unitPrice");
            Contract.Requires<ArgumentNullException>(order != null, "order");
            Contract.Requires<ArgumentNullException>(orderLine != null, "orderLine");
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
            subOrderLinesBundleTotalPrices = (IEnumerable<Price>)Enumerable.ToList<Price>(subOrderLinesBundleTotalPrices);
            return new Price(unitPrice.Value + Enumerable.Sum<Price>(subOrderLinesBundleTotalPrices, (Func<Price, Decimal>)(p => p.Value)), unitPrice.Vat + Enumerable.Sum<Price>(subOrderLinesBundleTotalPrices, (Func<Price, Decimal>)(p => p.Vat)), unitPrice.WithVat + Enumerable.Sum<Price>(subOrderLinesBundleTotalPrices, (Func<Price, Decimal>)(p => p.WithVat)), currency);
        }

        protected virtual Price CalculateOrderLineBundleTotalPrice(Price bundleUnitPrice, OrderLine orderLine, Currency currency, Order order)
        {
            Contract.Requires<ArgumentNullException>(bundleUnitPrice != null, "bundleUnitPrice");
            Contract.Requires<ArgumentNullException>(order != null, "order");
            Contract.Requires<ArgumentNullException>(orderLine != null, "orderLine");
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
            return new Price(bundleUnitPrice.Value * orderLine.Quantity, bundleUnitPrice.Vat * orderLine.Quantity, bundleUnitPrice.WithVat * orderLine.Quantity, currency);
        }

        protected void CalculateSubtotals(Order order)
        {
            Contract.Requires<ArgumentNullException>(order != null, "order");
            order.SubtotalPrices = new PriceCollection();
            if (order.OrderLines.Count > 0)
            {
                foreach (IGrouping<long, Price> grouping in Enumerable.GroupBy<Price, long>(Enumerable.SelectMany<OrderLine, Price>((IEnumerable<OrderLine>)order.OrderLines, (Func<OrderLine, IEnumerable<Price>>)(ol => (IEnumerable<Price>)ol.BundleTotalPrices)), (Func<Price, long>)(p => p.CurrencyId)))
                {
                    BookKeeping.Domain.Models.Currency currency = this.CurrencyService.Get(order.StoreId, grouping.Key);
                    order.SubtotalPrices.Add(this.CalculateOrderSubtotalPrice((IEnumerable<Price>)grouping, currency, order));
                }
            }
            else
            {
                foreach (BookKeeping.Domain.Models.Currency currency in this.CurrencyService.GetAll(order.StoreId))
                    order.SubtotalPrices.Add(new Price(new Decimal(0), order.VatRate, currency));
            }
            order.SubtotalPrice = order.SubtotalPrices.Get(order.CurrencyId);
        }

        protected virtual Price CalculateOrderSubtotalPrice(IEnumerable<Price> orderLinesBundleTotalPrices, BookKeeping.Domain.Models.Currency currency, Order order)
        {
            orderLinesBundleTotalPrices = (IEnumerable<Price>)Enumerable.ToList<Price>(orderLinesBundleTotalPrices);
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
            Contract.Requires<ArgumentNullException>(order != null, "order");
            Decimal num = Enumerable.Sum<Price>(orderLinesBundleTotalPrices, (Func<Price, Decimal>)(p => p.Value));
            Decimal vat = Enumerable.Sum<Price>(orderLinesBundleTotalPrices, (Func<Price, Decimal>)(p => p.Vat));
            return new Price(num, vat, num + vat, currency);
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
                foreach (BookKeeping.Domain.Models.Currency currency in this.CurrencyService.GetAll(order.StoreId))
                    order.ShipmentInformation.TotalPrices.Add(new Price(new Decimal(0), order.ShipmentInformation.VatRate, currency));
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
                foreach (BookKeeping.Domain.Models.Currency currency in this.CurrencyService.GetAll(order.StoreId))
                    order.PaymentInformation.TotalPrices.Add(new Price(new Decimal(0), order.PaymentInformation.VatRate, currency));
            }
            order.PaymentInformation.TotalPrice = order.PaymentInformation.TotalPrices.Get(order.CurrencyId);
        }

        protected void CalculateTransactionCosts(Order order)
        {
            Contract.Requires<ArgumentNullException>(order != null, "order");
            order.TransactionInformation.TransactionFee = new PriceWithoutVat(this.CalculateOrderTransactionFee(order), this.CurrencyService.Get(order.StoreId, order.CurrencyId));
        }

        protected virtual Decimal CalculateOrderTransactionFee(Order order)
        {
            Contract.Requires<ArgumentNullException>(order != null, "order");
            return order.TransactionInformation.TransactionFee.Value;
        }

        protected void CalculateTotals(Order order)
        {
            Contract.Requires<ArgumentNullException>(order != null, "order");
            order.TotalPrices = new PriceCollection();
            foreach (Price subtotalPrice in (List<Price>)order.SubtotalPrices)
            {
                BookKeeping.Domain.Models.Currency currency = this.CurrencyService.Get(order.StoreId, subtotalPrice.CurrencyId);
                Price shipmentTotalPrice = order.ShipmentInformation.TotalPrices.Get(subtotalPrice.CurrencyId);
                Price paymentTotalPrice = order.PaymentInformation.TotalPrices.Get(subtotalPrice.CurrencyId);
                PriceWithoutVat transactionFee = order.TransactionInformation.TransactionFee.CurrencyId == subtotalPrice.CurrencyId ? order.TransactionInformation.TransactionFee : new PriceWithoutVat(new Decimal(0), currency);
                order.TotalPrices.Add(this.CalculateOrderTotalPrice(subtotalPrice, shipmentTotalPrice, paymentTotalPrice, transactionFee, currency, order));
            }
            order.TotalPrice = order.TotalPrices.Get(order.CurrencyId);
        }

        protected virtual Price CalculateOrderTotalPrice(Price subtotalPrice, Price shipmentTotalPrice, Price paymentTotalPrice, PriceWithoutVat transactionFee, BookKeeping.Domain.Models.Currency currency, Order order)
        {
            Contract.Requires<ArgumentNullException>(subtotalPrice != null, "subtotalPrice");
            Contract.Requires<ArgumentNullException>(shipmentTotalPrice != null, "shipmentTotalPrice");
            Contract.Requires<ArgumentNullException>(paymentTotalPrice != null, "paymentTotalPrice");
            Contract.Requires<ArgumentNullException>(transactionFee != null, "transactionFee");
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
            Contract.Requires<ArgumentNullException>(order != null, "order");
            Decimal num = subtotalPrice.Value + shipmentTotalPrice.Value + paymentTotalPrice.Value + transactionFee.Value;
            Decimal vat = subtotalPrice.Vat + shipmentTotalPrice.Vat + paymentTotalPrice.Vat;
            return new Price(num, vat, num + vat, currency);
        }
    }
}