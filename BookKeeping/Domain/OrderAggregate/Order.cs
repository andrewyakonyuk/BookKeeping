using BookKeeping.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Domain.OrderAggregate
{
    public sealed class Order
    {
        public readonly IList<IEvent> _changes = new List<IEvent>();

        private readonly OrderState _state;

        public Order(IEnumerable<IEvent> events)
        {
            _state = new OrderState(events);
        }

        private void Apply(IEvent e)
        {
            _state.Mutate(e);
            _changes.Add(e);
        }

        public IList<IEvent> Changes { get { return _changes; } }
    }
}
