using System.Collections.Generic;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.PriceCalculators
{
    public interface IShippingCalculator
    {
        VatRate CalculateVatRate(ShippingMethod shippingMethod, Order order);

        IEnumerable<Price> CalculatePrices(ShippingMethod shippingMethod, Order order);

        VatRate CalculateVatRate(ShippingMethod shippingMethod, long countryId, long? countryRegionId, VatRate fallbackVatRate);

        IEnumerable<Price> CalculatePrices(ShippingMethod shippingMethod, long countryId, long? countryRegionId, VatRate vatRate);
    }
}