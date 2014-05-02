using BookKeeping.Core.Domain;
using BookKeeping.Domain.Contracts;
using System.Collections.Generic;

namespace BookKeeping.Domain.OrderAggregate
{
    public sealed class OrderState
    {
        readonly List<OrderLine> _orderLines = new List<OrderLine>();

        public OrderState(IEnumerable<IEvent> events)
        {
            foreach (var e in events)
            {
                Mutate(e);
            }
        }

        public void Mutate(IEvent e)
        {
            ((dynamic)this).When((dynamic)e);
        }

        public List<OrderLine> Lines { get { return _orderLines; } }

        public CurrencyAmount Total { get; private set; }

        public CurrencyAmount TotalWithVat { get; private set; }
    }
}
