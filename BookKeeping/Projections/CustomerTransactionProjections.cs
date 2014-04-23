using BookKeeping.Core;
using BookKeeping.Domain;
using BookKeeping.Domain.CustomerAggregate;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BookKeeping.Projections
{
    [DataContract]
    [Serializable]
    public class CustomerTransactionDto
    {
        public CurrencyAmount Balance { get; set; }
        public CurrencyAmount Change { get; set; }
        public string Name { get; set; }
        public DateTime TimeUtc { get; set; }
    }

    [DataContract]
    [Serializable]
    public class CustomerTransactionsDto
    {
        public IList<CustomerTransactionDto> Transactions = new System.Collections.ObjectModel.ObservableCollection<CustomerTransactionDto>();

        public void AddTx(string name, CurrencyAmount change, CurrencyAmount balance, DateTime timeUtc)
        {
            Transactions.Add(new CustomerTransactionDto
            {
                Name = name,
                Balance = balance,
                Change = change,
                TimeUtc = timeUtc
            });
        }
    }

}