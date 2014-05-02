using BookKeeping.Core;
using BookKeeping.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Domain.Contracts
{
    [Serializable]
    public sealed class CreateOrder : ICommand<OrderId>
    {
        public OrderId Id { get; set; }
        public CustomerId Customer { get; set; }

        public override string ToString()
        {
            return string.Format("Create {0} for {1}", Id, Customer);
        }
    }

    [Serializable]
    public sealed class OrderCreated : ICommand<OrderId>
    {
        public OrderId Id { get; set; }
        public CustomerId Customer { get; set; }
    }

    [Serializable]
    public sealed class AddOrderLine : ICommand<OrderId>
    {
        public OrderId Id { get; set; }
        public string ItemNo { get; set; }
        public string Title { get; set; }
        public CurrencyAmount Amount { get; set; }
        public double Quantity { get; set; }

        public override string ToString()
        {
            return string.Format("Add line to {0}", Id);
        }
    }

    [Serializable]
    public sealed class OrderLineAdded : IEvent<OrderId>
    {
        public OrderId Id { get; set; }
        public int Line { get; set; }
        public string ItemNo { get; set; }
        public string Title { get; set; }
        public CurrencyAmount Amount { get; set; }
        public double Quantity { get; set; }

        public override string ToString()
        {
            return string.Format("Line {0} was added to {1}", Line, Id);
        }
    }
}
