using BookKeeping.UI.ViewModels;
using System.Linq;

namespace BookKeeping.App.ViewModels
{
    public class AddressViewModel : ViewModelBase
    {
        public AddressViewModel()
        {
            this.PropertyChanged += AddressViewModel_PropertyChanged;
        }

        void AddressViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var fields = new[] { GetPropertyName(() => Country), GetPropertyName(() => City), 
                GetPropertyName(() => Street), GetPropertyName(() => ZipCode) };
            if (fields.Contains(e.PropertyName))
            {
                HasChanges = true;
            }
        }

        private string _Country;

        public string Country
        {
            get { return _Country; }
            set
            {
                _Country = value;
                OnPropertyChanged(() => Country);
            }
        }

        private string _City;

        public string City
        {
            get { return _City; }
            set
            {
                _City = value;
                OnPropertyChanged(() => City);
            }
        }
        private string _ZipCode;

        public string ZipCode
        {
            get { return _ZipCode; }
            set
            {
                _ZipCode = value;
                OnPropertyChanged(() => ZipCode);
            }
        }

        private string _Street;

        public string Street
        {
            get { return _Street; }
            set
            {
                _Street = value;
                OnPropertyChanged(() => Street);
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
        
    }
}
