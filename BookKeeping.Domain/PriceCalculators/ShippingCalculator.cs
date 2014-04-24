using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using BookKeeping.Domain.Models;
using BookKeeping.Domain.Services;

namespace BookKeeping.Domain.PriceCalculators
{
    public class ShippingCalculator : IShippingCalculator
    {
        protected readonly IVatGroupService VatGroupService;
        protected readonly ICurrencyService CurrencyService;

        public ShippingCalculator(IVatGroupService vatGroupService, ICurrencyService currencyService)
        {
            this.VatGroupService = vatGroupService;
            this.CurrencyService = currencyService;
        }

        public virtual VatRate CalculateVatRate(ShippingMethod shippingMethod, Order order)
        {
            Contract.Requires<ArgumentNullException>(shippingMethod != null, "shippingMethod");
            Contract.Requires<ArgumentNullException>(order != null, "order");
            return this.CalculateVatRate(shippingMethod, order.ShipmentInformation.CountryId ?? order.PaymentInformation.CountryId, order.ShipmentInformation.CountryId.HasValue ? order.ShipmentInformation.CountryRegionId : order.PaymentInformation.CountryRegionId, order.VatRate);
        }

        public IEnumerable<Price> CalculatePrices(ShippingMethod shippingMethod, Order order)
        {
            Contract.Requires<ArgumentNullException>(shippingMethod != null, "shippingMethod");
            Contract.Requires<ArgumentNullException>(order != null, "order");
            return (IEnumerable<Price>)Enumerable.ToList<Price>(Enumerable.Select<BookKeeping.Domain.Models.Currency, Price>(this.CurrencyService.GetAll(shippingMethod.StoreId), (Func<BookKeeping.Domain.Models.Currency, Price>)(currency => new Price(this.CalculatePrice(shippingMethod, currency, order), order.ShipmentInformation.VatRate, currency))));
        }

        protected virtual Decimal CalculatePrice(ShippingMethod shippingMethod, BookKeeping.Domain.Models.Currency currency, Order order)
        {
            Contract.Requires<ArgumentNullException>(shippingMethod != null, "shippingMethod");
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
            Contract.Requires<ArgumentNullException>(order != null, "order");
            return this.CalculatePrice(shippingMethod, currency, order.ShipmentInformation.CountryId ?? order.PaymentInformation.CountryId, order.ShipmentInformation.CountryId.HasValue ? order.ShipmentInformation.CountryRegionId : order.PaymentInformation.CountryRegionId);
        }

        public virtual VatRate CalculateVatRate(ShippingMethod shippingMethod, long countryId, long? countryRegionId, VatRate fallbackVatRate)
        {
            Contract.Requires<ArgumentNullException>(shippingMethod != null, "shippingMethod");
            Contract.Requires<ArgumentNullException>(fallbackVatRate != null, "fallbackVatRate");
            VatRate vatRate = fallbackVatRate;
            if (shippingMethod.VatGroupId.HasValue)
                vatRate = this.VatGroupService.Get(shippingMethod.StoreId, shippingMethod.VatGroupId.Value).GetVatRate(countryId, countryRegionId);
            return vatRate;
        }

        public IEnumerable<Price> CalculatePrices(ShippingMethod shippingMethod, long countryId, long? countryRegionId, VatRate vatRate)
        {
            Contract.Requires<ArgumentNullException>(shippingMethod != null, "shippingMethod");
            Contract.Requires<ArgumentNullException>(vatRate != null, "vatRate");
            return (IEnumerable<Price>)Enumerable.ToList<Price>(Enumerable.Select<BookKeeping.Domain.Models.Currency, Price>(this.CurrencyService.GetAll(shippingMethod.StoreId), (Func<BookKeeping.Domain.Models.Currency, Price>)(currency => new Price(this.CalculatePrice(shippingMethod, currency, countryId, countryRegionId), vatRate, currency))));
        }

        protected virtual Decimal CalculatePrice(ShippingMethod shippingMethod, BookKeeping.Domain.Models.Currency currency, long countryId, long? countryRegionId)
        {
            Contract.Requires<ArgumentNullException>(shippingMethod != null, "shippingMethod");
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
            return shippingMethod.GetOriginalPrice(currency.Id, countryId, countryRegionId);
        }
    }
}