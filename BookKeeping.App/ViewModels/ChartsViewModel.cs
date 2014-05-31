using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using BookKeeping.UI;
using BookKeeping.UI.ViewModels;
using System.Windows.Media;
using BookKeeping.App.Exporters;

namespace BookKeeping.App.ViewModels
{
    public class ChartsViewModel : WorkspaceViewModel, IPrintable
    {
        readonly PdfExporter _exporter = new PdfExporter();

        public DelegateCommand AddSeriesCommand { get; set; }
        public ObservableCollection<string> ChartTypes { get; set; }
        public List<double> FontSizes { get; set; }
        public List<double> DoughnutInnerRadiusRatios { get; set; }
        public Dictionary<string, De.TorstenMandelkow.MetroChart.ResourceDictionaryCollection> Palettes { get; set; }
        public List<string> SelectionBrushes { get; set; }
        
        private string selectedChartType = null;
        public string SelectedChartType 
        {
            get
            {
                return selectedChartType;
            }
            set
            {
                selectedChartType = value;
                OnPropertyChanged("SelectedChartType");
            }
        }

        private object selectedPalette = null;
        public object SelectedPalette
        {
            get
            {
                return selectedPalette;
            }
            set
            {
                selectedPalette = value;
                OnPropertyChanged("SelectedPalette");
            }
        }

        private bool darkLayout = false;
        public bool DarkLayout
        {
            get
            {
                return darkLayout;
            }
            set
            {
                darkLayout = value;
                OnPropertyChanged("DarkLayout");
                OnPropertyChanged("Foreground");
                OnPropertyChanged("Background");
                OnPropertyChanged("MainBackground");
                OnPropertyChanged("MainForeground");                
            }
        }

        public string Foreground 
        { 
            get
            {
                if (darkLayout)
                {
                    return "#FFEEEEEE";      
                }
                return "#FF666666";  
            }
        }
        public string MainForeground
        {
            get
            {
                if (darkLayout)
                {
                    return "#FFFFFFFF";
                }
                return "#FF666666";
            }
        }
        public string Background
        {
            get
            {
                if (darkLayout)
                {
                    return "#FF333333";
                }
                return "#FFF9F9F9";
            }
        }
        public string MainBackground
        {
            get
            {
                if (darkLayout)
                {
                    return "#FF000000";
                }
                return "#FFEFEFEF";
            }
        }
        
        
        private string selectedBrush = null;
        public string SelectedBrush
        {
            get
            {
                return selectedBrush;
            }
            set
            {
                selectedBrush = value;
                OnPropertyChanged("SelectedBrush");
            }
        }

        private double selectedDoughnutInnerRadiusRatio = 0.75;
        public double SelectedDoughnutInnerRadiusRatio
        {
            get
            {
                return selectedDoughnutInnerRadiusRatio;
            }
            set
            {
                selectedDoughnutInnerRadiusRatio = value;
                OnPropertyChanged("SelectedDoughnutInnerRadiusRatio");
                OnPropertyChanged("SelectedDoughnutInnerRadiusRatioString");
            }
        }

        public string SelectedDoughnutInnerRadiusRatioString
        {
            get
            {
                return String.Format("{0:P1}.", SelectedDoughnutInnerRadiusRatio);
            }
        }

        public ChartsViewModel()
        {
            DisplayName = T("Charts");

            LoadPalettes();

            AddSeriesCommand = new DelegateCommand(x => AddSeries());

            ChartTypes = new ObservableCollection<string>();
            ChartTypes.Add("All");            
            ChartTypes.Add("Column");
            ChartTypes.Add("StackedColumn");
            ChartTypes.Add("Bar");
            ChartTypes.Add("StackedBar");
            ChartTypes.Add("Pie");
            ChartTypes.Add("Doughnut");
            ChartTypes.Add("Gauge");            
            SelectedChartType = ChartTypes.FirstOrDefault();

            FontSizes = new List<double>();
            FontSizes.Add(9.0);
            FontSizes.Add(11.0);
            FontSizes.Add(13.0);
            FontSizes.Add(18.0);
            SelectedFontSize = 11.0;

            DoughnutInnerRadiusRatios = new List<double>();
            DoughnutInnerRadiusRatios.Add(0.90);
            DoughnutInnerRadiusRatios.Add(0.75);
            DoughnutInnerRadiusRatios.Add(0.5);
            DoughnutInnerRadiusRatios.Add(0.25);
            DoughnutInnerRadiusRatios.Add(0.1);
            SelectedDoughnutInnerRadiusRatio = 0.75;

            SelectionBrushes = new List<string>();
            SelectionBrushes.Add("Orange");
            SelectionBrushes.Add("Red");
            SelectionBrushes.Add("Yellow");
            SelectionBrushes.Add("Blue");
            SelectionBrushes.Add("[NoColor]");
            SelectedBrush = SelectionBrushes.FirstOrDefault();

            Series = new ObservableCollection<SeriesData>();

            Errors = new ObservableCollection<TestClass>();
            var random = new Random();
            for (int i = 0; i < 12; i++)
            {
                Errors.Add(new TestClass
                {
                    Category = new string("qwertyuiopasdfghjklzxcvbnm".Substring(random.Next(0, 12)).OrderBy(t => Guid.NewGuid()).ToArray()),
                    Number =  random.Next(10, 100)
                });
            }
        }

        int newSeriesCounter = 1;
        private void AddSeries()
        {
            ObservableCollection<TestClass> data = new ObservableCollection<TestClass>();

            data.Add(new TestClass() { Category = "Globalization", Number = 5 });
            data.Add(new TestClass() { Category = "Features", Number = 10 });
            data.Add(new TestClass() { Category = "ContentTypes", Number = 15 });
            data.Add(new TestClass() { Category = "Correctness", Number = 20 });
            data.Add(new TestClass() { Category = "Naming", Number = 15 });
            data.Add(new TestClass() { Category = "Best Practices", Number = 10 });

            Series.Add(new SeriesData() { SeriesDisplayName = "New Series " + newSeriesCounter.ToString(), Items = data });

            newSeriesCounter++;
        }

        private void LoadPalettes()
        {
            Palettes = new Dictionary<string, De.TorstenMandelkow.MetroChart.ResourceDictionaryCollection>();
            Palettes.Add("Default", null);

            var resources = Application.Current.Resources.MergedDictionaries.ToList();
            foreach (var dict in resources)
            {
                foreach (var objkey in dict.Keys)
                {
                    if(dict[objkey] is De.TorstenMandelkow.MetroChart.ResourceDictionaryCollection)
                    {
                        Palettes.Add(objkey.ToString(), dict[objkey] as De.TorstenMandelkow.MetroChart.ResourceDictionaryCollection);
                    }
                }
            }

            SelectedPalette = Palettes.FirstOrDefault();
        }

        private bool isRowColumnSwitched = false;
        public bool IsRowColumnSwitched
        {
            get
            {
                return isRowColumnSwitched;
            }
            set
            {
                isRowColumnSwitched = value;
                OnPropertyChanged("IsRowColumnSwitched");
            }
        }

        private bool isLegendVisible = true;
        public bool IsLegendVisible
        {
            get
            {
                return isLegendVisible;
            }
            set
            {
                isLegendVisible = value;
                OnPropertyChanged("IsLegendVisible");
            }
        }

        private bool isTitleVisible = true;
        public bool IsTitleVisible
        {
            get
            {
                return isTitleVisible;
            }
            set
            {
                isTitleVisible = value;
                OnPropertyChanged("IsTitleVisible");
            }
        }

        private double fontSize = 11.0;
        public double SelectedFontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                fontSize = value;
                OnPropertyChanged(()=>SelectedFontSize);
                OnPropertyChanged(()=>SelectedFontSizeString);
            }
        }

        public string SelectedFontSizeString
        {
            get
            {
                return SelectedFontSize.ToString() + "px";
            }
        }

        private object selectedItem = null;
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

        public ObservableCollection<SeriesData> Series
        {
            get;
            set;
        }

        public ObservableCollection<TestClass> Errors
        {
            get;
            set;
        }

        public ObservableCollection<TestClass> Warnings
        {
            get;
            set;
        }

        public string ToolTipFormat
        {
            get
            {
                return "{0} in series '{2}' has value '{1}' ({3:P2})";
            }           
        }

        public void Print()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.DefaultExt = ".pdf";

            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                _exporter.Export(Visual, saveFileDialog.FileName);
            }
        }

        public Visual Visual { get; set; }
    }

    public class SeriesData
    {
        public string SeriesDisplayName { get; set; }

        public string SeriesDescription { get; set; }

        public ObservableCollection<TestClass> Items { get; set; }
    }

    public class TestClass : INotifyPropertyChanged
    {
        public string Category { get; set; }

        private float _number = 0;
        public float Number 
        {
            get
            {
                return _number;
            }
            set
            {
                _number = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Number"));
                }
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
