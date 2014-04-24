using System;
using System.Collections.Generic;
using System.Xml.Linq;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.Repositories
{
    public interface IOrderRepository
    {
        Guid? GetOrderId(long storeId, string cartNumber);

        XElement GetAllFinalizedAsXml(long storeId);

        Order Get(long storeId, Guid orderId);

        IEnumerable<Order> Get(long storeId, IEnumerable<Guid> orderIds);

        Tuple<IEnumerable<Order>, long> Search(long storeId, long orderStatusId, string orderNumber, string firstName, string lastName, PaymentState? paymentState, DateTime? startDate, DateTime? endDate, bool? isFinalized, long page, long itemsPerPage);

        void Save(Order order);

        void Delete(Order order);
    }
}