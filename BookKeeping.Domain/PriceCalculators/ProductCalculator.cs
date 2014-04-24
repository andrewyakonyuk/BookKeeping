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
            return (IEnumerable<Price>)Enumerable.ToList<Price>(Enumerable.Select(Enumerable.Select((IEnumerable<OriginalUnitPrice>)this.ProductService.GetOriginalUnitPrices(productIdentifier), originalUnitPrice => new
            {
                originalUnitPrice = originalUnitPrice,
                currency = this.CurrencyService.Get(order.StoreId, originalUnitPrice.CurrencyId)
            }), param0 => new Price(this.CalculatePrice(param0.originalUnitPrice, productIdentifier, param0.currency, order), vatRate, param0.currency)));
        }

        protected virtual Decimal CalculatePrice(OriginalUnitPrice originalUnitPrice, string productIdentifier, Currency currency, Order order)
        {
            Contract.Requires<ArgumentNullException>(originalUnitPrice != null, "originalUnitPrice");
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
            Contract.Requires<ArgumentNullException>(order != null, "order");
            return this.CalculatePrice(originalUnitPrice, productIdentifier, currency);
        }

        public virtual VatRate CalculateVatRate(string productIdentifier, long countryId, long? countryRegionId, VatRate fallbackVatRate)
        {
            Contract.Requires<ArgumentNullException>(fallbackVatRate != null, "fallbackVatRate");
            VatRate vatRate = fallbackVatRate;
            long? vatGroupId = this.ProductService.GetVatGroupId(productIdentifier);
            if (vatGroupId.HasValue)
                vatRate = this.VatGroupService.Get(this.ProductService.GetStoreId(productIdentifier), vatGroupId.Value).GetVatRate(countryId, countryRegionId);
            return vatRate;
        }

        public IEnumerable<Price> CalculatePrices(string productIdentifier, VatRate vatRate)
        {
            Contract.Requires<ArgumentNullException>(vatRate != null, "vatRate");
            long storeId = this.ProductService.GetStoreId(productIdentifier);
            return (IEnumerable<Price>)Enumerable.ToList<Price>(Enumerable.Select(Enumerable.Select((IEnumerable<OriginalUnitPrice>)this.ProductService.GetOriginalUnitPrices(productIdentifier), originalUnitPrice => new
            {
                originalUnitPrice = originalUnitPrice,
                currency = this.CurrencyService.Get(storeId, originalUnitPrice.CurrencyId)
            }), param0 => new Price(this.CalculatePrice(param0.originalUnitPrice, productIdentifier, param0.currency), vatRate, param0.currency)));
        }

        protected virtual Decimal CalculatePrice(OriginalUnitPrice originalUnitPrice, string productIdentifier, BookKeeping.Domain.Models.Currency currency)
        {
            Contract.Requires<ArgumentNullException>(originalUnitPrice != null, "originalUnitPrice");
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
            return originalUnitPrice.Value;
        }
    }
}