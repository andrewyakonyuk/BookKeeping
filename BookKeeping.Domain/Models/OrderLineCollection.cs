using System;
using System.Collections.Generic;
using System.Linq;
using BookKeeping.Domain.Services;

namespace BookKeeping.Domain.Models
{
    public class OrderLineCollection : List<OrderLine>, ICopyable<OrderLineCollection>
    {
        public IEnumerable<OrderLine> GetAll()
        {
            List<OrderLine> list = Enumerable.ToList<OrderLine>((IEnumerable<OrderLine>)this);
            foreach (OrderLine orderLine in (List<OrderLine>)this)
                list.AddRange(orderLine.OrderLines.GetAll());
            return (IEnumerable<OrderLine>)list;
        }

        public OrderLine Get(long orderLineId)
        {
            OrderLine orderLine1 = Enumerable.SingleOrDefault<OrderLine>((IEnumerable<OrderLine>)this, (Func<OrderLine, bool>)(ol =>
            {
                if (ol.Id == orderLineId)
                    return true;
                long? local_0 = ol.CopiedFromOrderLineId;
                long local_1 = orderLineId;
                if (local_0.GetValueOrDefault() == local_1)
                    return local_0.HasValue;
                else
                    return false;
            }));
            if (orderLine1 == null)
            {
                foreach (OrderLine orderLine2 in (List<OrderLine>)this)
                {
                    orderLine1 = orderLine2.OrderLines.Get(orderLineId);
                    if (orderLine1 != null)
                        break;
                }
            }
            return orderLine1;
        }

        public OrderLine GetUnique(ProductSnapshot productSnapshot, IDictionary<string, string> properties = null)
        {
            IList<string> uniquenessPropertyAliases = StoreService.Instance.Get(productSnapshot.StoreId).ProductUniquenessPropertyAliases;
            OrderLine orderLine;
            if (!Enumerable.Any<string>((IEnumerable<string>)uniquenessPropertyAliases))
            {
                orderLine = Enumerable.SingleOrDefault<OrderLine>((IEnumerable<OrderLine>)this, (Func<OrderLine, bool>)(ol => ol.Sku == productSnapshot.Sku));
            }
            else
            {
                IDictionary<string, string> uniquenessProperties = (IDictionary<string, string>)new Dictionary<string, string>();
                foreach (string index in (IEnumerable<string>)uniquenessPropertyAliases)
                {
                    CustomProperty customProperty = productSnapshot.Properties.Get(index);
                    if (customProperty != null)
                        uniquenessProperties.Add(customProperty.Alias, customProperty.Value);
                    else if (properties != null && properties.ContainsKey(index))
                        uniquenessProperties.Add(index, properties[index]);
                }
                uniquenessProperties = (IDictionary<string, string>)Enumerable.ToDictionary<KeyValuePair<string, string>, string, string>(Enumerable.Where<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>)uniquenessProperties, (Func<KeyValuePair<string, string>, bool>)(kvp => !string.IsNullOrEmpty(kvp.Value))), (Func<KeyValuePair<string, string>, string>)(kvp => kvp.Key), (Func<KeyValuePair<string, string>, string>)(kvp => kvp.Value));
                orderLine = Enumerable.SingleOrDefault<OrderLine>((IEnumerable<OrderLine>)this, (Func<OrderLine, bool>)(ol =>
                {
                    if (ol.Sku == productSnapshot.Sku)
                        return Enumerable.All<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>)uniquenessProperties, (Func<KeyValuePair<string, string>, bool>)(p => Enumerable.Any<CustomProperty>((IEnumerable<CustomProperty>)ol.Properties, (Func<CustomProperty, bool>)(p2 =>
                        {
                            if (p2.Alias == p.Key)
                                return p2.Value == p.Value;
                            else
                                return false;
                        }))));
                    else
                        return false;
                }));
            }
            return orderLine;
        }

        public OrderLine GetBundle(string bundleIdentifier)
        {
            OrderLine orderLine1 = Enumerable.SingleOrDefault<OrderLine>((IEnumerable<OrderLine>)this, (Func<OrderLine, bool>)(ol => ol.BundleIdentifier == bundleIdentifier));
            if (orderLine1 == null)
            {
                foreach (OrderLine orderLine2 in (List<OrderLine>)this)
                {
                    orderLine1 = orderLine2.OrderLines.GetBundle(bundleIdentifier);
                    if (orderLine1 != null)
                        break;
                }
            }
            return orderLine1;
        }

        public OrderLine Remove(long orderLineId)
        {
            OrderLine orderLine1 = Enumerable.SingleOrDefault<OrderLine>((IEnumerable<OrderLine>)this, (Func<OrderLine, bool>)(ol =>
            {
                if (ol.Id == orderLineId)
                    return true;
                long? local_0 = ol.CopiedFromOrderLineId;
                long local_1 = orderLineId;
                if (local_0.GetValueOrDefault() == local_1)
                    return local_0.HasValue;
                else
                    return false;
            }));
            if (orderLine1 == null)
            {
                foreach (OrderLine orderLine2 in (List<OrderLine>)this)
                {
                    orderLine1 = orderLine2.OrderLines.Remove(orderLineId);
                    if (orderLine1 != null)
                        break;
                }
            }
            else
                base.Remove(orderLine1);
            return orderLine1;
        }

        public OrderLine AddOrUpdate(string productIdentifier, Decimal? quantity = null, IDictionary<string, string> properties = null, bool overwriteQuantity = false, string bundleIdentifier = null, string parentBundleIdentifier = null)
        {
            OrderLine orderLine1 = (OrderLine)null;
            OrderLine orderLine2 = !string.IsNullOrEmpty(parentBundleIdentifier) ? this.GetBundle(parentBundleIdentifier) : (OrderLine)null;
            ProductSnapshot snapshot = ProductService.Instance.GetSnapshot(productIdentifier);
            if (string.IsNullOrEmpty(parentBundleIdentifier))
                orderLine1 = this.GetUnique(snapshot, properties);
            else if (orderLine2 != null)
                orderLine1 = orderLine2.OrderLines.GetUnique(snapshot, properties);
            if ((quantity.HasValue || properties != null && Enumerable.Any<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>)properties)) && (orderLine1 == null && (string.IsNullOrEmpty(parentBundleIdentifier) || orderLine2 != null)))
            {
                orderLine1 = new OrderLine(snapshot)
                {
                    BundleIdentifier = bundleIdentifier
                };
                if (string.IsNullOrEmpty(parentBundleIdentifier))
                    this.Add(orderLine1);
                else if (orderLine2 != null)
                    orderLine2.OrderLines.Add(orderLine1);
            }
            this.Update(orderLine1, quantity, properties, overwriteQuantity);
            return orderLine1;
        }

        public OrderLine Update(long orderLineId, Decimal? quantity = null, IDictionary<string, string> properties = null, bool overwriteQuantity = false)
        {
            OrderLine orderLine = this.Get(orderLineId);
            this.Update(orderLine, quantity, properties, overwriteQuantity);
            return orderLine;
        }

        private void Update(OrderLine orderLine, Decimal? quantity, IDictionary<string, string> properties, bool overwriteQuantity)
        {
            if (orderLine == null)
                return;
            if (quantity.HasValue)
                orderLine.ChangeQuantity(!overwriteQuantity ? orderLine.Quantity + quantity.Value : quantity.Value);
            orderLine.Properties.AddOrUpdate(properties);
        }

        public OrderLineCollection Copy()
        {
            OrderLineCollection orderLineCollection = new OrderLineCollection();
            orderLineCollection.AddRange(Enumerable.Select<OrderLine, OrderLine>((IEnumerable<OrderLine>)this, (Func<OrderLine, OrderLine>)(orderLine => orderLine.Copy())));
            return orderLineCollection;
        }

        public override bool Equals(object obj)
        {
            OrderLineCollection orderLineCollection = obj as OrderLineCollection;
            if (orderLineCollection == null)
                return false;
            bool flag = this.Count == orderLineCollection.Count;
            if (flag)
                flag = !Enumerable.Any<OrderLine>((IEnumerable<OrderLine>)this, (Func<OrderLine, bool>)(ol => Enumerable.All<OrderLine>((IEnumerable<OrderLine>)orderLineCollection, (Func<OrderLine, bool>)(col => !col.Equals((object)ol)))));
            return flag;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}