using BookKeeping.Domain;
using BookKeeping.UI.ViewModels;

namespace BookKeeping.App.ViewModels
{
    public class ProductViewModel : ViewModelBase
    {
        public ProductViewModel()
        {
            this.PropertyChanged += ProductViewModel_PropertyChanged;
        }

        void ProductViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == GetPropertyName(() => HasChanges) 
                || e.PropertyName == GetPropertyName(() => IsHighlight)
                || e.PropertyName == GetPropertyName(()=>IsValid))
                return;
            HasChanges = true;
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged(() => Title);
            }
        }

        private Barcode _barcode;

        public Barcode Barcode
        {
            get { return _barcode; }
            set
            {
                _barcode = value;
                OnPropertyChanged(() => Barcode);
            }
        }

        private string _itemNo;

        public string ItemNo
        {
            get { return _itemNo; }
            set
            {
                _itemNo = value;
                OnPropertyChanged(() => ItemNo);
            }
        }
        private CurrencyAmount _price;

        public CurrencyAmount Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged(() => Price);
            }
        }

        private decimal _stock;

        public decimal Stock
        {
            get { return _stock; }
            set
            {
                _stock = value;
                OnPropertyChanged(() => Stock);
            }
        }

        private string _unitOfMeasure;

        public string UnitOfMeasure
        {
            get { return _unitOfMeasure; }
            set
            {
                _unitOfMeasure = value;
                OnPropertyChanged(() => UnitOfMeasure);
            }
        }

        private VatRate _vatRate;

        public VatRate VatRate
        {
            get { return _vatRate; }
            set
            {
                _vatRate = value;
                OnPropertyChanged(() => VatRate);
            }
        }

        private bool _isOrderable;

        public bool IsOrderable
        {
            get { return _isOrderable; }
            set
            {
                _isOrderable = value;
                OnPropertyChanged(() => IsOrderable);
            }
        }

        private bool _hasChanges;

        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                _hasChanges = value;
                OnPropertyChanged(() => HasChanges);
            }
        }
        

        protected override string GetErrorMessage(string columnName)
        {
            switch (columnName)
            {
                case "Stock":
                    if (Stock < 0)
                    {
                        IsValid = false;
                        return T("ShouldBeMoreOrEqualZero");
                    }
                    break;
                case "VatRate":
                    if (VatRate < 0M)
                    {
                        IsValid = false;
                        return T("ShouldBeMoreOrEqualZero");
                    }
                    break;
                case "Price":
                    if (Price.Amount < 0)
                    {
                        IsValid = false;
                        return T("ShouldBeMoreOrEqualZero");
                    }
                    if (Price.Currency == Currency.Undefined)
                    {
                        IsValid = false;
                        return T("CurrencyShouldBeNotUndefined");
                    }
                    break;
                case "ItemNo":
                    if (string.IsNullOrWhiteSpace(ItemNo))
                    {
                        IsValid = false;
                        return T("ShouldBeNotEmpty");
                    }
                    break;
                case "Barcode":
                    if (Barcode.Type == BarcodeType.Undefined)
                    {
                        IsValid = false;
                        return T("BarcodeShouldBeNotUndefined");
                    }
                    break;
                default:
                    return null;
            }
            IsValid = true;
            return null;
        }

        private bool _isHighlight;

        public bool IsHighlight
        {
            get { return _isHighlight; }
            set
            {
                _isHighlight = value;
                OnPropertyChanged(() => IsHighlight);
            }
        }
        
    }
}
