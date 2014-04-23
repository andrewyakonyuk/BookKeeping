using BookKeeping.Domain.CustomerAggregate;
using BookKeeping.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.App.ViewModels
{
    public class CustomerTransactionsViewModel : WorkspaceViewModel
    {
        public CustomerTransactionsViewModel()
        {
            DisplayName = "Customer transitions";

            //var reader = Context.Current.ViewDocs.GetReader<CustomerId, CustomerTransactionsDto>();
            //var transactions = reader.Get(new CustomerId(12));
            Source = new System.Collections.ObjectModel.ObservableCollection<CustomerTransactionDto>();
        }
    }
}
