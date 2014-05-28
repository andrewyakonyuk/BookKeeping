using BookKeeping.Domain.Contracts;

namespace BookKeeping.Domain.Order
{
    public sealed class OrderLine
    {
        public int Id { get; set; }
        public string ItemNo { get; set; }
        public string Title { get; set; }
        public double Quantity { get; set; }
        public CurrencyAmount Amount { get; set; }
    }
}
