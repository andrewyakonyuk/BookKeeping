using System.Collections.Generic;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.PriceCalculators
{
    public interface IProductCalculator
    {
        VatRate CalculateVatRate(string productIdentifier, Order order);

        IEnumerable<Price> CalculatePrices(string productIdentifier, Order order, VatRate vatRate);

        VatRate CalculateVatRate(string productIdentifier, long countryId, long? countryRegionId, VatRate fallbackVatRate);

        IEnumerable<Price> CalculatePrices(string productIdentifier, VatRate vatRate);
    }
}