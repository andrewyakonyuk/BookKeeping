using BookKeeping.App.Domain.Aggregates;

namespace BookKeeping.App.Domain.Services
{
    public interface IOrderCalculator
    {
        OrderCalculationResult CalculateOrder(Order order);
    }

    public struct OrderCalculationResult
    {
        public readonly CurrencyAmount TotalPrice;
        public readonly CurrencyAmount TotalPriceInclVat;
        public readonly VatRate VatRate;

        public OrderCalculationResult(CurrencyAmount totalPrice, CurrencyAmount totalPriceInclVat, VatRate vatRate)
        {
            TotalPrice = totalPrice;
            TotalPriceInclVat = totalPriceInclVat;
            VatRate = vatRate;
        }
    }
}
