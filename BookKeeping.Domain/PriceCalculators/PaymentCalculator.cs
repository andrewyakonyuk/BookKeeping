using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using BookKeeping.Domain.Models;
using BookKeeping.Domain.Services;

namespace BookKeeping.Domain.PriceCalculators
{
    public class PaymentCalculator : IPaymentCalculator
    {
        protected readonly IVatGroupService VatGroupService;
        protected readonly ICurrencyService CurrencyService;

        public PaymentCalculator(IVatGroupService vatGroupService, ICurrencyService currencyService)
        {
            this.VatGroupService = vatGroupService;
            this.CurrencyService = currencyService;
        }

        public virtual VatRate CalculateVatRate(PaymentMethod paymentMethod, Order order)
        {
            Contract.Requires<ArgumentNullException>(paymentMethod != null, "paymentMethod");
            Contract.Requires<ArgumentNullException>(order != null, "order");
            return this.CalculateVatRate(paymentMethod, order.PaymentInformation.CountryId, order.PaymentInformation.CountryRegionId, order.VatRate);
        }

        public IEnumerable<Price> CalculatePrices(PaymentMethod paymentMethod, Order order)
        {
            Contract.Requires<ArgumentNullException>(paymentMethod != null, "paymentMethod");
            Contract.Requires<ArgumentNullException>(order != null, "order");
            return (IEnumerable<Price>)Enumerable.ToList<Price>(Enumerable.Select<BookKeeping.Domain.Models.Currency, Price>(this.CurrencyService.GetAll(paymentMethod.StoreId), (Func<BookKeeping.Domain.Models.Currency, Price>)(currency => new Price(this.CalculatePrice(paymentMethod, currency, order), order.PaymentInformation.VatRate, currency))));
        }

        protected virtual Decimal CalculatePrice(PaymentMethod paymentMethod, BookKeeping.Domain.Models.Currency currency, Order order)
        {
            Contract.Requires<ArgumentNullException>(paymentMethod != null, "paymentMethod");
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
            Contract.Requires<ArgumentNullException>(order != null, "order");
            return this.CalculatePrice(paymentMethod, currency, order.PaymentInformation.CountryId, order.PaymentInformation.CountryRegionId);
        }

        public virtual VatRate CalculateVatRate(PaymentMethod paymentMethod, long countryId, long? countryRegionId, VatRate fallbackVatRate)
        {
            Contract.Requires<ArgumentNullException>(paymentMethod != null, "paymentMethod");
            Contract.Requires<ArgumentNullException>(fallbackVatRate != null, "fallbackVatRate");
            VatRate vatRate = fallbackVatRate;
            if (paymentMethod.VatGroupId.HasValue)
                vatRate = this.VatGroupService.Get(paymentMethod.StoreId, paymentMethod.VatGroupId.Value).GetVatRate(countryId, countryRegionId);
            return vatRate;
        }

        public IEnumerable<Price> CalculatePrices(PaymentMethod paymentMethod, long countryId, long? countryRegionId, VatRate vatRate)
        {
            Contract.Requires<ArgumentNullException>(paymentMethod != null, "paymentMethod");
            Contract.Requires<ArgumentNullException>(vatRate != null, "vatRate");
            return (IEnumerable<Price>)Enumerable.ToList<Price>(Enumerable.Select<BookKeeping.Domain.Models.Currency, Price>(this.CurrencyService.GetAll(paymentMethod.StoreId), (Func<BookKeeping.Domain.Models.Currency, Price>)(currency => new Price(this.CalculatePrice(paymentMethod, currency, countryId, countryRegionId), vatRate, currency))));
        }

        protected virtual Decimal CalculatePrice(PaymentMethod paymentMethod, BookKeeping.Domain.Models.Currency currency, long countryId, long? countryRegionId)
        {
            Contract.Requires<ArgumentNullException>(paymentMethod != null, "paymentMethod");
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
            return paymentMethod.GetOriginalPrice(currency.Id, countryId, countryRegionId);
        }
    }
}