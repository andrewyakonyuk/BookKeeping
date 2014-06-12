using BookKeeping.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BookKeeping.Projections.CustomerTransactions
{
    [DataContract]
    [Serializable]
    public class CustomerTransactionView
    {
        [DataMember(Order = 1)]
        public CurrencyAmount Balance { get; set; }
        [DataMember(Order = 2)]
        public CurrencyAmount Change { get; set; }
        [DataMember(Order = 3)]
        public string Name { get; set; }
        [DataMember(Order = 4)]
        public DateTime TimeUtc { get; set; }
    }

    [DataContract]
    [Serializable]
    public class CustomerTransactionsListView
    {
        [DataMember(Order = 4)]
        public IList<CustomerTransactionView> Transactions = new List<CustomerTransactionView>();

        public void AddTx(string name, CurrencyAmount change, CurrencyAmount balance, DateTime timeUtc)
        {
            Transactions.Add(new CustomerTransactionView
            {
                Name = name,
                Balance = balance,
                Change = change,
                TimeUtc = timeUtc
            });
        }
    }
}
