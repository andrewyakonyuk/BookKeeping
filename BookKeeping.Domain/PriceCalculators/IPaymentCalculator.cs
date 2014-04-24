using System.Collections.Generic;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.PriceCalculators
{
    public interface IPaymentCalculator
    {
        VatRate CalculateVatRate(PaymentMethod paymentMethod, Order order);

        IEnumerable<Price> CalculatePrices(PaymentMethod paymentMethod, Order order);

        VatRate CalculateVatRate(PaymentMethod paymentMethod, long countryId, long? countryRegionId, VatRate fallbackVatRate);

        IEnumerable<Price> CalculatePrices(PaymentMethod paymentMethod, long countryId, long? countryRegionId, VatRate vatRate);
    }
}