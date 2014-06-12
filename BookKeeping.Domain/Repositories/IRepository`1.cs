using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;
using BookKeeping.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Domain.Repositories
{
    public interface IRepository<TAggregate, TKey>
        where TAggregate : AggregateBase
        where TKey : IIdentity
    {
        IEnumerable<TAggregate> All();

        TAggregate Get(TKey id);

        TAggregate Load(TKey id);
    }
}
