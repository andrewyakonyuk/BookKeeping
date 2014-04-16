using BookKeeping.Core;
using BookKeeping.Core.AtomicStorage;
using BookKeeping.Domain;
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
        public IList<CustomerTransactionDto> Transactions = new List<CustomerTransactionDto>();

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

    public class CustomerTransactionsProjection : IEventHandler<CustomerCreated>,
        IEventHandler<CustomerChargeAdded>,
        IEventHandler<CustomerPaymentAdded>
    {
        private readonly IDocumentWriter<CustomerId, CustomerTransactionsDto> _store;

        public CustomerTransactionsProjection(IDocumentWriter<CustomerId, CustomerTransactionsDto> store)
        {
            _store = store;
        }

        public void When(CustomerCreated e)
        {
            _store.Add(e.Id, new CustomerTransactionsDto());
        }

        public void When(CustomerChargeAdded e)
        {
            _store.UpdateOrThrow(e.Id, v => v.AddTx(e.ChargeName, -e.Charge, e.NewBalance, e.TimeUtc));
        }

        public void When(CustomerPaymentAdded e)
        {
            _store.UpdateOrThrow(e.Id, v => v.AddTx(e.PaymentName, e.Payment, e.NewBalance, e.TimeUtc));
        }
    }
}