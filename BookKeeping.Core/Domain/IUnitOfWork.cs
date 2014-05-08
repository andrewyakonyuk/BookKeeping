using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Core.Domain
{
    public interface IUnitOfWork
    {
        void Commit();
        void Rollback();
    }
}
