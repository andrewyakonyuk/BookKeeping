using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;

namespace BookKeeping.Domain.Services
{
    public interface IOrderCalculator
    {
        OrderCalculationResult CalculateOrder(Order.Order order);
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
