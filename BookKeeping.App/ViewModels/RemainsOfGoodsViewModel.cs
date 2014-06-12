using BookKeeping.App.Exporters;
using BookKeeping.App.Views;
using BookKeeping.UI;
using BookKeeping.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace BookKeeping.App.ViewModels
{
    public class RemainsOfGoodsViewModel : WorkspaceViewModel, IPrintable
    {
        readonly PdfExporter _exporter = new PdfExporter();

        public RemainsOfGoodsViewModel()
        {
            DisplayName = T("RemainsOfGoods");
        }

        public void Print()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.DefaultExt = ".pdf";

            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                _exporter.Export(DocumentPaginator, saveFileDialog.FileName);
            }
        }

        public DocumentPaginator DocumentPaginator { get; set; }
    }
}
