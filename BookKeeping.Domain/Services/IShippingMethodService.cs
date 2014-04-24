using System.Collections.Generic;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.Services
{
    public interface IShippingMethodService
    {
        IEnumerable<ShippingMethod> GetAll(long storeId);

        IEnumerable<ShippingMethod> GetAllAllowedIn(long storeId, long countryId, long? countryRegionId = null);

        ShippingMethod Get(long storeId, long shippingMethodId);
    }
}