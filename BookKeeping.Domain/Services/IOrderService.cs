using System;
using System.Collections.Generic;
using System.Xml.Linq;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.Services
{
    public interface IOrderService
    {
        long? GetOrderId(long storeId, string cartNumber);

        Order Get(long storeId, long orderId);

        IEnumerable<Order> Get(long storeId, IEnumerable<long> orderIds);

        Tuple<IEnumerable<Order>, long> Search(long storeId, long orderStatusId, string orderNumber, string firstName, string lastName, PaymentState? paymentState, DateTime? startDate, DateTime? endDate, bool? isFinalized, long page, long itemsPerPage);
    }
}