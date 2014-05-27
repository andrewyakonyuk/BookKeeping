using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Domain.Repositories
{
    public interface IRepository<TEntity>
        where TEntity : IEntity
    {
        IEnumerable<TEntity> All();

        TEntity Get(long id);

        void Save(TEntity entity);
    }
}
