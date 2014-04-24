using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Infrastructure.Dependency
{
    public interface IDependencyResolver
    {
        object GetService(Type serviceType);
        IEnumerable<object> GetServices(Type serviceType);
    }
}
