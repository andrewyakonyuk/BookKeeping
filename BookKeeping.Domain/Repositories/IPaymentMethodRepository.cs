using System.Collections.Generic;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.Repositories
{
    public interface IPaymentMethodRepository
    {
        IEnumerable<PaymentMethod> GetAll(long storeId);

        PaymentMethod Get(long storeId, long shippingMethodId);

        void Save(PaymentMethod paymentMethod);

        int GetHighestSortValue(long storeId);
    }
}