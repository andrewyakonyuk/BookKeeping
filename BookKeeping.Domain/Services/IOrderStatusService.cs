using System.Collections.Generic;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.Services
{
    public interface IOrderStatusService
    {
        IEnumerable<OrderStatus> GetAll(long storeId);

        OrderStatus Get(long storeId, long orderStatusId);
    }
}