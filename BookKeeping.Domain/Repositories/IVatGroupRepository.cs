using System.Collections.Generic;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.Repositories
{
    public interface IVatGroupRepository
    {
        IEnumerable<VatGroup> GetAll(long storeId);

        VatGroup Get(long storeId, long vatGroupId);

        void Save(VatGroup vatGroup);

        int GetHighestSortValue(long storeId);
    }
}