using System.Collections.Generic;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.Notifications
{
    public delegate void OrderPropertiesEventHandler(BookKeeping.Domain.Models.Order order, IEnumerable<CustomProperty> properties);
}