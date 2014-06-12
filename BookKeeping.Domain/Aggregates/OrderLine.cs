using BookKeeping.Domain.Contracts;

namespace BookKeeping.Domain.Aggregates
{
    public sealed class OrderLine
    {
        public long Id { get; set; }

        public ProductId ProductId { get; set; }

        public string ItemNo { get; set; }

        public string Title { get; set; }

        public decimal Quantity { get; set; }

        public CurrencyAmount Amount { get; set; }

        public VatRate VatRate { get; set; }
    }
}