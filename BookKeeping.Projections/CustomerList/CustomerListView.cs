using BookKeeping.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BookKeeping.Projections.CustomerList
{
    [DataContract]
    [Serializable]
    public class CustomerView
    {
        [DataMember(Order = 1)]
        public CustomerId Id { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public CurrencyAmount Balance { get; set; }

        [DataMember(Order = 4)]
        public bool ManualBilling { get; set; }

        [DataMember(Order = 5)]
        public Address LegalAddress { get; set; }

        [DataMember(Order = 6)]
        public string BankingDetails { get; set; }

        [DataMember(Order = 7)]
        public string Phone { get; set; }

        [DataMember(Order = 8)]
        public string Fax { get; set; }

        [DataMember(Order = 9)]
        public string Email { get; set; }

        [DataMember(Order = 10)]
        public Currency Currency { get; set; }
    }

    [DataContract]
    [Serializable]
    public class CustomerListView
    {
        private List<CustomerView> _customers = new List<CustomerView>();

        [DataMember(Order = 1)]
        public List<CustomerView> Customers { get { return _customers; } }
    }
}
