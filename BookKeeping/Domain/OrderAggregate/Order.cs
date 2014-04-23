using BookKeeping.Core;
using BookKeeping.Domain.ProductAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Domain.OrderAggregate
{
    public class Order
    {
        readonly List<OrderLine> _orderLines = new List<OrderLine>();

        public Order()
        {

        }

        public IEnumerable<OrderLine> Lines { get { return _orderLines; } }

        public CurrencyAmount Total { get; protected set; }

        public CurrencyAmount TotalWithVat { get; protected set; }

        public void AddLine(OrderLine line)
        {
            Total = new CurrencyAmount(Total.Amount + (line.Amount.Amount * (decimal)line.Quantity), Total.Currency);
        }

        public void AddLine(Product product, double quantity)
        {
            var line = new OrderLine
            {
                Id = _orderLines.Count + 1,
                ItemNo = product.ItemNo,
                Title = product.Title,
                Quantity = quantity
            };
            _orderLines.Add(line);
            Total = new CurrencyAmount(Total.Amount + (product.Price.Amount * (decimal)quantity), product.Price.Currency);
        }
    }
}
