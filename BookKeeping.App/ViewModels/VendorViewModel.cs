
namespace BookKeeping.App.ViewModels
{
    public class VendorViewModel : ListItemViewModel
    {
        public VendorViewModel()
        {
            LegalAddress = new AddressViewModel();
        }

        private string _FullName;

        public string FullName
        {
            get { return _FullName; }
            set
            {
                _FullName = value;
                OnPropertyChanged(() => FullName);
            }
        }

        private AddressViewModel _LegalAddress;

        public AddressViewModel LegalAddress
        {
            get { return _LegalAddress; }
            set
            {
                _LegalAddress = value;
                OnPropertyChanged(() => LegalAddress);
            }
        }

        private string _Phone;

        public string Phone
        {
            get { return _Phone; }
            set
            {
                _Phone = value;
                OnPropertyChanged(() => Phone);
            }
        }

        private string _Fax;

        public string Fax
        {
            get { return _Fax; }
            set
            {
                _Fax = value;
                OnPropertyChanged(() => Fax);
            }
        }

        private string _Email;

        public string Email
        {
            get { return _Email; }
            set
            {
                _Email = value;
                OnPropertyChanged(() => Email);
            }
        }

        protected override string GetErrorMessage(string columnName)
        {
            string errorMessage = null;
            switch (columnName)
            {
                case "FullName":
                    if (string.IsNullOrWhiteSpace(FullName))
                        return T("ShouldBeNotEmpty");
                    break;
                default:
                    break;
            }
            return errorMessage;
        }
    }
}
