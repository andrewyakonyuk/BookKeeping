using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BookKeeping.App.Exporters
{
    public interface IExporter
    {
        void Export(Stream source, string fileName);
    }
}
