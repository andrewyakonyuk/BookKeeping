using BookKeeping.Domain.Contracts;
using System.Collections.Generic;

namespace BookKeeping.Domain.Aggregates
{
    public abstract class AggregateBase
    {
        private readonly IList<IEvent> _changes = new List<IEvent>();

        protected AggregateBase(IEnumerable<IEvent> events)
        {
            foreach (var e in events)
            {
                Mutate(e);
            }
        }

        public virtual long Version { get; protected set; }

        public virtual IList<IEvent> Changes { get { return _changes; } }

        protected virtual void Apply(IEvent e)
        {
            this.Mutate(e);
            _changes.Add(e);
        }

        protected virtual void Mutate(IEvent e)
        {
            Version += 1;
            ((dynamic)this).When((dynamic)e);
        }
    }
}
