using System.Collections.Generic;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.Repositories
{
    public interface IShippingMethodRepository
    {
        IEnumerable<ShippingMethod> GetAll(long storeId);

        ShippingMethod Get(long storeId, long shippingMethodId);

        void Save(ShippingMethod shippingMethod);

        int GetHighestSortValue(long storeId);
    }
}