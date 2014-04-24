using System.Collections.Generic;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.Services
{
    public interface IVatGroupService
    {
        IEnumerable<VatGroup> GetAll(long storeId);

        VatGroup Get(long storeId, long vatGroupId);
    }
}