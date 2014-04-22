using BookKeeping.Domain.CustomerAggregate;
using BookKeeping.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookKeeping.Core.AtomicStorage;

namespace BookKeeping.App.ViewModels
{
    public class CustomerTransactionsViewModel : WorkspaceViewModel
    {
        public CustomerTransactionsViewModel()
        {
            var reader = Context.Current.ViewDocs.GetReader<CustomerId, CustomerTransactionsDto>();
            var transactions = reader.Get(new CustomerId(12));
            DisplayName = "Customer transitions";
            Source = transactions.Convert(t => t.Transactions,
                () => new List<CustomerTransactionDto>());
        }
    }
}
