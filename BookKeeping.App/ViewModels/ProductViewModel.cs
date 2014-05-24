using BookKeeping.Domain;
using BookKeeping.UI.ViewModels;

namespace BookKeeping.App.ViewModels
{
   public class ProductViewModel : ViewModelBase
    {
        private long _id;

        public long Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(() => Id);
            }
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
        
    }
}
