using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using BookKeeping.Domain.Models;
using BookKeeping.Domain.Services;

namespace BookKeeping.Domain.PriceCalculators
{
    public class ProductCalculator : IProductCalculator
    {
        protected readonly IVatGroupService VatGroupService;
        protected readonly ICurrencyService CurrencyService;
        protected readonly IProductService ProductService;

        public ProductCalculator(IVatGroupService vatGroupService, ICurrencyService currencyService, IProductService productService)
        {
            this.ProductService = productService;
            this.CurrencyService = currencyService;
            this.VatGroupService = vatGroupService;
        }

        public virtual VatRate CalculateVatRate(string productIdentifier, Order order)
        {
            Contract.Requires<ArgumentNullException>(order != null, "order");
            return this.CalculateVatRate(productIdentifier, order.PaymentInformation.CountryId, order.PaymentInformation.CountryRegionId, order.VatRate);
        }

        public IEnumerable<Price> CalculatePrices(string productIdentifier, Order order, VatRate vatRate)
        {
            Contract.Requires<ArgumentNullException>(order != null, "order");
            Contract.Requires<ArgumentNullException>(vatRate != null, "vatRate");
            return (
                from originalUnitPrice in this.ProductService.GetOriginalUnitPrices(productIdentifier)
                let currency = this.CurrencyService.Get(order.StoreId, originalUnitPrice.CurrencyId)
                select new Price(this.CalculatePrice(originalUnitPrice, productIdentifier, currency, order), vatRate, currency)).ToList<Price>();
        }

        protected virtual decimal CalculatePrice(OriginalUnitPrice originalUnitPrice, string productIdentifier, Currency currency, Order order)
        {
            Contract.Requires<ArgumentNullException>(originalUnitPrice != null, "originalUnitPrice");
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
            Contract.Requires<ArgumentNullException>(order != null, "order");
            return this.CalculatePrice(originalUnitPrice, productIdentifier, currency);
        }

        public virtual VatRate CalculateVatRate(string productIdentifier, long countryId, long? countryRegionId, VatRate fallbackVatRate)
        {
            Contract.Requires<ArgumentNullException>(fallbackVatRate != null, "fallbackVatRate");
            VatRate result = fallbackVatRate;
            long? vatGroupId = this.ProductService.GetVatGroupId(productIdentifier);
            if (vatGroupId.HasValue)
            {
                result = this.VatGroupService.Get(this.ProductService.GetStoreId(productIdentifier), vatGroupId.Value).GetVatRate(countryId, countryRegionId);
            }
            return result;
        }

        public IEnumerable<Price> CalculatePrices(string productIdentifier, VatRate vatRate)
        {
            Contract.Requires<ArgumentNullException>(vatRate != null, "vatRate");
            long storeId = this.ProductService.GetStoreId(productIdentifier);
            return (
                from originalUnitPrice in this.ProductService.GetOriginalUnitPrices(productIdentifier)
                let currency = this.CurrencyService.Get(storeId, originalUnitPrice.CurrencyId)
                select new Price(this.CalculatePrice(originalUnitPrice, productIdentifier, currency), vatRate, currency)).ToList<Price>();
        }

        protected virtual decimal CalculatePrice(OriginalUnitPrice originalUnitPrice, string productIdentifier, Currency currency)
        {
            Contract.Requires<ArgumentNullException>(originalUnitPrice != null, "originalUnitPrice");
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
            return originalUnitPrice.Value;
        }
    }
}