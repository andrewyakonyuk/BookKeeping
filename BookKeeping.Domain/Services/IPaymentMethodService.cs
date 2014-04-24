using System.Collections.Generic;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.Services
{
    public interface IPaymentMethodService
    {
        IEnumerable<PaymentMethod> GetAll(long storeId);

        IEnumerable<PaymentMethod> GetAllAllowedIn(long storeId, long countryId, long? countryRegionId = null);

        PaymentMethod Get(long storeId, long paymentMethodId);
    }
}