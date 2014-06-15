using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.App
{
    public interface ISessionFactory
    {
        ISession Create();
    }
}
