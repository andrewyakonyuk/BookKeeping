using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.UI
{
    public interface ISaveable
    {
        void SaveChanges();
        bool CanSave { get; }
    }
}
