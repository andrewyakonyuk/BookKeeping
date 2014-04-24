using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.PriceCalculators
{
    public interface IOrderCalculator
    {
        void CalculateOrder(Order order);
    }
}