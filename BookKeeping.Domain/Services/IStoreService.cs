using System.Collections.Generic;
using BookKeeping.Domain.Models;

namespace BookKeeping.Domain.Services
{
    public interface IStoreService
    {
        IEnumerable<Store> GetAll();

        Store Get(long storeId);
    }
}