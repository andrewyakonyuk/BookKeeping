using System.Collections.Generic;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.Repositories
{
    public interface IStoreRepository
    {
        IEnumerable<Store> GetAll();

        Store Get(long storeId);

        void Save(Store store);

        int GetHighestSortValue();
    }
}