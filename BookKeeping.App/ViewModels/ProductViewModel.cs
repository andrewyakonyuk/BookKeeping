using BookKeeping.Domain.Contracts;
using BookKeeping.UI.ViewModels;

namespace BookKeeping.App.ViewModels
{
    public class ProductViewModel : ListItemViewModel
    {
        int _countOfErrors = 0;

        public ProductViewModel()
        {
            Price = CurrencyAmount.Unspecifined;
            Barcode = Barcode.Undefined;
            VatRate = VatRate.Zero;
            Title = string.Empty;
            ItemNo = string.Empty;
            UnitOfMeasure = string.Empty;
            IsOrderable = true;
            IsValid = false;
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

        protected override string GetErrorMessage(string columnName)
        {
            string error = null;
            switch (columnName)
            {
                case "Stock":
                    if (Stock < 0)
                    {
                        _countOfErrors++;
                        error = T("ShouldBeMoreOrEqualZero");
                    }
                    else _countOfErrors--;
                    break;
                case "VatRate":
                    if (VatRate < 0M)
                    {
                        _countOfErrors++;
                        error = T("ShouldBeMoreOrEqualZero");
                    }
                    else _countOfErrors--;
                    break;
                case "Price":
                    if (Price.Amount < 0)
                    {
                        _countOfErrors++;
                        error = T("ShouldBeMoreOrEqualZero");
                    }
                    else if (Price.Currency == Currency.Undefined)
                    {
                        _countOfErrors++;
                        error = T("CurrencyShouldBeNotUndefined");
                    }
                    else _countOfErrors--;
                    break;
                case "ItemNo":
                    if (string.IsNullOrWhiteSpace(ItemNo))
                    {
                        _countOfErrors++;
                        error = T("ShouldBeNotEmpty");
                    }
                    else _countOfErrors--;
                    break;
                case "Barcode":
                    if (Barcode.Type == BarcodeType.Undefined)
                    {
                        _countOfErrors++;
                        error = T("BarcodeShouldBeNotUndefined");
                    }
                    else _countOfErrors--;
                    break;
                default:
                    error = null;
                    break;
            }
            return error;
        }
    }
}
