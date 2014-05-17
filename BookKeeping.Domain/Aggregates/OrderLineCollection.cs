using System.Collections.Generic;
using System.Linq;

namespace BookKeeping.Domain.Aggregates
{
    public class OrderLineCollection : List<OrderLine>
    {
        public OrderLine Get(long orderLineId)
        {
           return this.SingleOrDefault((OrderLine ol) => ol.Id == orderLineId);
        }

        public OrderLine GetUnique(Product product)
        {
            return this.SingleOrDefault((OrderLine ol) => ol.ItemNo == product.ItemNo);
        }

        public OrderLine Remove(long orderLineId)
        {
            OrderLine orderLine = this.SingleOrDefault((OrderLine ol) => ol.Id == orderLineId);
            base.Remove(orderLine);
            return orderLine;
        }

        public OrderLine AddOrUpdate(Product product, decimal? quantity = null, bool overwriteQuantity = false)
        {
            OrderLine orderLine = this.GetUnique(product);
            if (orderLine == null)
            {
                orderLine = new OrderLine(product, quantity.HasValue ? (decimal)quantity : 1);
                this.Add(orderLine);
            }
            else
            {
                Update(orderLine, quantity, overwriteQuantity);
            }
            return orderLine;
        }

        public OrderLine Update(long orderLineId, decimal? quantity = null, bool overwriteQuantity = false)
        {
            OrderLine orderLine = this.Get(orderLineId);
            Update(orderLine, quantity, overwriteQuantity);
            return orderLine;
        }

        static void Update(OrderLine orderLine, decimal? quantity, bool overwriteQuantity)
        {
            if (orderLine == null)
            {
                return;
            }
            if (quantity.HasValue)
            {
                orderLine.Quantity = overwriteQuantity ? quantity.Value : orderLine.Quantity + quantity.Value;
            }
        }

        public override bool Equals(object obj)
        {
            OrderLineCollection orderLineCollection = obj as OrderLineCollection;
            if (orderLineCollection == null)
            {
                return false;
            }
            return base.Count == orderLineCollection.Count
                && this.Any(line => orderLineCollection.All((OrderLine col) => col.Equals(line)));
        }

        public override int GetHashCode()
        {
            //TODO: impl hashcode
            return base.GetHashCode();
        }
    }
}
