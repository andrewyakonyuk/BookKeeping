using System.Collections.Generic;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.Notifications
{
    public delegate void OrderLinesEventHandler(BookKeeping.Domain.Models.Order order, IEnumerable<OrderLine> orderLines);
}