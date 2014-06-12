using BookKeeping.App.Exporters;
using BookKeeping.UI;
using BookKeeping.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace BookKeeping.App.ViewModels
{
    public class SalesForPeriodViewModel : WorkspaceViewModel, IPrintable
    {
        readonly PdfExporter _exporter = new PdfExporter();
        private bool isRowColumnSwitched = false;
        private bool isLegendVisible = true;
        private bool isTitleVisible = true;
        private object selectedItem = null;
        private string _subTitle;
        private string _title;

        public SalesForPeriodViewModel()
        {
            DisplayName = T("SalesForPeriod");

            Title = DisplayName;
            SubTitle = string.Empty;

            Monthes = new ObservableCollection<SalesForMonthData>();
            var months = new[] { T("January"), T("February"), T("March"), T("April"), T("May"), T("June"), T("July"), T("August"), T("September"), T("October"), T("November"), T("December") };
            var random = new Random();
            for (int i = 0; i < 12; i++)
            {
                Monthes.Add(new SalesForMonthData
                {
                    Month = months[i],
                    Sales = random.Next(10, 100)
                });
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged(() => Title);
            }
        }

        public string SubTitle
        {
            get { return _subTitle; }
            set
            {
                _subTitle = value;
                OnPropertyChanged(() => SubTitle);
            }
        }

        public bool IsRowColumnSwitched
        {
            get
            {
                return isRowColumnSwitched;
            }
            set
            {
                isRowColumnSwitched = value;
                OnPropertyChanged(()=>IsRowColumnSwitched);
            }
        }

        public bool IsLegendVisible
        {
            get
            {
                return isLegendVisible;
            }
            set
            {
                isLegendVisible = value;
                OnPropertyChanged(()=>IsLegendVisible);
            }
        }

        public bool IsTitleVisible
        {
            get
            {
                return isTitleVisible;
            }
            set
            {
                isTitleVisible = value;
                OnPropertyChanged(() => IsTitleVisible);
            }
        }

        public object SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                OnPropertyChanged(()=>SelectedItem);
            }
        }

        public ObservableCollection<SalesForMonthData> Monthes
        {
            get;
            set;
        }

        public string ToolTipFormat
        {
            get
            {
                return T("SalesForPeriod_ToolTipFormat");
            }
        }

        public void Print()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.DefaultExt = ".pdf";

            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                _exporter.Export(PrintArea, saveFileDialog.FileName);
            }
        }

        public Visual PrintArea { get; set; }
    }

    public class SalesForMonthData : NotificationObject
    {
        public string Month { get; set; }

        private float _number = 0;
        public float Sales
        {
            get
            {
                return _number;
            }
            set
            {
                _number = value;
                OnPropertyChanged(() => Sales);
            }
        }
    }
}
