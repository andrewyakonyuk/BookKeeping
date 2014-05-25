using BookKeeping.UI.Localization;
using MahApps.Metro.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.UI.Converters
{
    public abstract class MarkupConverterBase : MarkupConverter
    {
        protected MarkupConverterBase()
        {
            T = ResourceLocalizer.Instance;
        }

        protected Localizer T { get; set; }
    }
}
