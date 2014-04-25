using System.Collections.Generic;
using System.Linq;
using BookKeeping.Domain.Services;

namespace BookKeeping.Domain.Models
{
    public class OrderLineCollection : List<OrderLine>, ICopyable<OrderLineCollection>
    {
        public IEnumerable<OrderLine> GetAll()
        {
            List<OrderLine> list = this.ToList<OrderLine>();
            foreach (OrderLine current in this)
            {
                list.AddRange(current.OrderLines.GetAll());
            }
            return list;
        }

        public OrderLine Get(long orderLineId)
        {
            OrderLine orderLine = this.SingleOrDefault((OrderLine ol) => ol.Id == orderLineId || ol.CopiedFromOrderLineId == orderLineId);
            if (orderLine == null)
            {
                foreach (OrderLine current in this)
                {
                    orderLine = current.OrderLines.Get(orderLineId);
                    if (orderLine != null)
                    {
                        break;
                    }
                }
            }
            return orderLine;
        }

        public OrderLine GetUnique(ProductSnapshot productSnapshot, IDictionary<string, string> properties = null)
        {
            IList<string> productUniquenessPropertyAliases = StoreService.Instance.Get(productSnapshot.StoreId).ProductUniquenessPropertyAliases;
            OrderLine result;
            if (!productUniquenessPropertyAliases.Any<string>())
            {
                result = this.SingleOrDefault((OrderLine ol) => ol.Sku == productSnapshot.Sku);
            }
            else
            {
                IDictionary<string, string> uniquenessProperties = new Dictionary<string, string>();
                foreach (string current in productUniquenessPropertyAliases)
                {
                    CustomProperty customProperty = productSnapshot.Properties.Get(current);
                    if (customProperty != null)
                    {
                        uniquenessProperties.Add(customProperty.Alias, customProperty.Value);
                    }
                    else
                    {
                        if (properties != null && properties.ContainsKey(current))
                        {
                            uniquenessProperties.Add(current, properties[current]);
                        }
                    }
                }
                uniquenessProperties = (
                    from kvp in uniquenessProperties
                    where !string.IsNullOrEmpty(kvp.Value)
                    select kvp).ToDictionary((KeyValuePair<string, string> kvp) => kvp.Key, (KeyValuePair<string, string> kvp) => kvp.Value);
                result = this.SingleOrDefault((OrderLine ol) => ol.Sku == productSnapshot.Sku && uniquenessProperties.All((KeyValuePair<string, string> p) => ol.Properties.Any((CustomProperty p2) => p2.Alias == p.Key && p2.Value == p.Value)));
            }
            return result;
        }

        public OrderLine GetBundle(string bundleIdentifier)
        {
            OrderLine orderLine = this.SingleOrDefault((OrderLine ol) => ol.BundleIdentifier == bundleIdentifier);
            if (orderLine == null)
            {
                foreach (OrderLine current in this)
                {
                    orderLine = current.OrderLines.GetBundle(bundleIdentifier);
                    if (orderLine != null)
                    {
                        break;
                    }
                }
            }
            return orderLine;
        }

        public OrderLine Remove(long orderLineId)
        {
            OrderLine orderLine = this.SingleOrDefault((OrderLine ol) => ol.Id == orderLineId || ol.CopiedFromOrderLineId == orderLineId);
            if (orderLine == null)
            {
                foreach (OrderLine item in this)
                {
                    orderLine = item.OrderLines.Remove(orderLineId);
                    if (orderLine != null)
                    {
                        break;
                    }
                    return orderLine;
                }
            }
            base.Remove(orderLine);
            return orderLine;
        }

        public OrderLine AddOrUpdate(string productIdentifier, decimal? quantity = null, IDictionary<string, string> properties = null, bool overwriteQuantity = false, string bundleIdentifier = null, string parentBundleIdentifier = null)
        {
            OrderLine orderLine = null;
            OrderLine orderLine2 = (!string.IsNullOrEmpty(parentBundleIdentifier)) ? this.GetBundle(parentBundleIdentifier) : null;
            ProductSnapshot snapshot = ProductService.Instance.GetSnapshot(productIdentifier);
            if (string.IsNullOrEmpty(parentBundleIdentifier))
            {
                orderLine = this.GetUnique(snapshot, properties);
            }
            else
            {
                if (orderLine2 != null)
                {
                    orderLine = orderLine2.OrderLines.GetUnique(snapshot, properties);
                }
            }
            if ((quantity.HasValue || (properties != null && properties.Any<KeyValuePair<string, string>>())) && orderLine == null && (string.IsNullOrEmpty(parentBundleIdentifier) || orderLine2 != null))
            {
                orderLine = new OrderLine(snapshot)
                {
                    BundleIdentifier = bundleIdentifier
                };
                if (string.IsNullOrEmpty(parentBundleIdentifier))
                {
                    base.Add(orderLine);
                }
                else
                {
                    if (orderLine2 != null)
                    {
                        orderLine2.OrderLines.Add(orderLine);
                    }
                }
            }
            this.Update(orderLine, quantity, properties, overwriteQuantity);
            return orderLine;
        }

        public OrderLine Update(long orderLineId, decimal? quantity = null, IDictionary<string, string> properties = null, bool overwriteQuantity = false)
        {
            OrderLine orderLine = this.Get(orderLineId);
            this.Update(orderLine, quantity, properties, overwriteQuantity);
            return orderLine;
        }

        private void Update(OrderLine orderLine, decimal? quantity, IDictionary<string, string> properties, bool overwriteQuantity)
        {
            if (orderLine == null)
            {
                return;
            }
            if (quantity.HasValue)
            {
                orderLine.ChangeQuantity((!overwriteQuantity) ? (orderLine.Quantity + quantity.Value) : quantity.Value);
            }
            orderLine.Properties.AddOrUpdate(properties);
        }

        public OrderLineCollection Copy()
        {
            OrderLineCollection orderLineCollection = new OrderLineCollection();
            orderLineCollection.AddRange(
                from orderLine in this
                select orderLine.Copy());
            return orderLineCollection;
        }

        public override bool Equals(object obj)
        {
            OrderLineCollection orderLineCollection = obj as OrderLineCollection;
            if (orderLineCollection == null)
            {
                return false;
            }
            bool flag = base.Count == orderLineCollection.Count;
            if (flag)
            {
                flag = !this.Any((OrderLine ol) => orderLineCollection.All((OrderLine col) => !col.Equals(ol)));
            }
            return flag;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}