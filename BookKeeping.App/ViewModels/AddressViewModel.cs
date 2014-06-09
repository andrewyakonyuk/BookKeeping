using BookKeeping.UI.ViewModels;

namespace BookKeeping.App.ViewModels
{
    public class AddressViewModel : ViewModelBase
    {
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
    }
}
