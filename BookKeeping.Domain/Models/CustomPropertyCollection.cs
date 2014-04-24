using System;
using System.Collections.Generic;
using System.Linq;

namespace BookKeeping.Domain.Models
{
    public class CustomPropertyCollection : List<CustomProperty>, ICopyable<CustomPropertyCollection>
    {
        public string this[string alias]
        {
            get
            {
                string str = "";
                CustomProperty customProperty = this.Get(alias);
                if (customProperty != null)
                    str = customProperty.Value;
                return str;
            }
        }

        public CustomProperty Get(string alias)
        {
            return Enumerable.SingleOrDefault<CustomProperty>((IEnumerable<CustomProperty>)this, (Func<CustomProperty, bool>)(p => p.Alias == alias));
        }

        public CustomProperty AddOrUpdate(string alias, string value)
        {
            this.AddOrUpdate(new CustomProperty(alias, value));
            return this.Get(alias);
        }

        public IEnumerable<CustomProperty> AddOrUpdate(IDictionary<string, string> properties)
        {
            if (properties != null && Enumerable.Any<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>)properties))
                this.AddOrUpdate(Enumerable.Select<KeyValuePair<string, string>, CustomProperty>((IEnumerable<KeyValuePair<string, string>>)properties, (Func<KeyValuePair<string, string>, CustomProperty>)(p => new CustomProperty(p.Key, p.Value))));
            return (IEnumerable<CustomProperty>)this;
        }

        public void AddOrUpdate(IEnumerable<CustomProperty> properties)
        {
            if (properties == null)
                return;
            foreach (CustomProperty property in properties)
                this.AddOrUpdate(property);
        }

        public void AddOrUpdate(CustomProperty property)
        {
            if (property == null)
                return;
            CustomProperty customProperty = this.Get(property.Alias);
            if (customProperty != null)
            {
                if (customProperty.IsReadOnly || customProperty.ServerSideOnly && customProperty.ServerSideOnly != property.ServerSideOnly)
                    return;
                customProperty.IsReadOnly = property.IsReadOnly;
                customProperty.ServerSideOnly = property.ServerSideOnly;
                customProperty.Value = property.Value;
            }
            else
                this.Add(property);
        }

        public CustomPropertyCollection Copy()
        {
            CustomPropertyCollection propertyCollection = new CustomPropertyCollection();
            propertyCollection.AddRange(Enumerable.Select<CustomProperty, CustomProperty>((IEnumerable<CustomProperty>)this, (Func<CustomProperty, CustomProperty>)(kvp => kvp.Copy())));
            return propertyCollection;
        }

        public override bool Equals(object obj)
        {
            CustomPropertyCollection customPropertyCollection = obj as CustomPropertyCollection;
            if (customPropertyCollection == null)
                return false;
            bool flag = this.Count == customPropertyCollection.Count;
            if (flag)
                flag = !Enumerable.Any<CustomProperty>((IEnumerable<CustomProperty>)this, (Func<CustomProperty, bool>)(p => Enumerable.All<CustomProperty>((IEnumerable<CustomProperty>)customPropertyCollection, (Func<CustomProperty, bool>)(cp => !cp.Equals((object)p)))));
            return flag;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}