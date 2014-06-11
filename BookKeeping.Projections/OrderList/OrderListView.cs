using BookKeeping.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace BookKeeping.Projections.OrdersList
{
    [DataContract]
    [Serializable]
    public class OrderView
    {
        public List<OrderLineView> Lines { get; private set; }

        public CurrencyAmount Total { get;  set; }

        public CurrencyAmount TotalWithVat { get;  set; }

        public VatRate VatRate { get;  set; }

        public OrderId Id { get;  set; }

        public CustomerId CustomerId { get;  set; }

        public bool IsCompleted { get;  set; }

        public decimal TotalQuantity { get; set; }

        public OrderView()
        {
            Lines = new List<OrderLineView>();
        }
    }

    [DataContract]
    [Serializable]
    public class OrderLineView
    {
        public long Id { get; set; }

        public ProductId ProductId { get; set; }

        public string ItemNo { get; set; }

        public string Title { get; set; }

        public decimal Quantity { get; set; }

        public CurrencyAmount Amount { get; set; }

        public VatRate VatRate { get; set; }
    }

    [DataContract]
    [Serializable]
    public class OrderListView
    {
        private List<OrderView> _products = new List<OrderView>();

        [DataMember(Order = 1)]
        public List<OrderView> Orders { get { return _products; } }
    }
}
