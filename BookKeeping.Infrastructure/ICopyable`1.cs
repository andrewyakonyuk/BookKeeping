using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Infrastructure
{
    public interface ICopyable<T>
    {
        T Copy();
    }
}
