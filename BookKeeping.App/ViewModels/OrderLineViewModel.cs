using BookKeeping.Domain.Contracts;
using BookKeeping.UI.ViewModels;

namespace BookKeeping.App.ViewModels
{
    public class OrderLineViewModel : ViewModelBase
    {
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

        private decimal _quantity;

        public decimal Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged(() => Quantity);
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

        private CurrencyAmount _amount;

        public CurrencyAmount Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                OnPropertyChanged(() => Amount);
            }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged(() => IsSelected);
            }
        }

        private long _productId;

        public long ProductId
        {
            get { return _productId; }
            set
            {
                _productId = value;
                OnPropertyChanged(() => ProductId);
            }
        }
        
    }
}
