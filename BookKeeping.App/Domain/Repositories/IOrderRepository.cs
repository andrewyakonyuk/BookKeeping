﻿using BookKeeping.App.Domain.Aggregates;
using System.Collections.Generic;

namespace BookKeeping.App.Domain.Repositories
{
    public interface IOrderRepository
    {
        Maybe<Order> Get(long orderId);
        IEnumerable<Order> GetAll();
    }
}
