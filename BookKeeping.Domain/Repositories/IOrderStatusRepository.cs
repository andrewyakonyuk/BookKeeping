using System.Collections.Generic;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.Repositories
{
    public interface IOrderStatusRepository
    {
        IEnumerable<OrderStatus> GetAll(long storeId);

        OrderStatus Get(long storeId, long orderStatusId);

        void Save(OrderStatus orderStatus);

        int GetHighestSortValue(long storeId);
    }
}