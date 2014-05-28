using BookKeeping.Domain.Aggregates;
using System.Linq;

namespace BookKeeping.Domain.Services
{
   public class OrderCalculator : IOrderCalculator
    {
        public OrderCalculationResult CalculateOrder(Order.Order order)
        {
            //if (order.OrderLines.Any())
            //{
            //    var currency = order.OrderLines.First().UnitPrice.Currency;
            //    var totalPrice = order.OrderLines.Aggregate(new CurrencyAmount(0, currency), (seed, line) => seed = seed + line.TotalPrice);
            //    var totalPriceInclVat = order.OrderLines.Aggregate(new CurrencyAmount(0, currency), (seed, line) => seed = seed + line.TotalPriceInclVat);
            //    var vatRate = (totalPriceInclVat - totalPrice).Amount / 100;
            //    return new OrderCalculationResult(totalPrice, totalPriceInclVat, new VatRate(vatRate));
            //}
            //else
            //{
            //    return new OrderCalculationResult(CurrencyAmount.Unspecifined, CurrencyAmount.Unspecifined, VatRate.Zero);
            //}
            return default(OrderCalculationResult);
        }
    }
}
