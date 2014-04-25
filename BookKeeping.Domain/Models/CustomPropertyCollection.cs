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
                string result = "";
                CustomProperty customProperty = this.Get(alias);
                if (customProperty != null)
                {
                    result = customProperty.Value;
                }
                return result;
            }
        }

        public CustomProperty Get(string alias)
        {
            return this.SingleOrDefault((CustomProperty p) => p.Alias == alias);
        }

        public CustomProperty AddOrUpdate(string alias, string value)
        {
            this.AddOrUpdate(new CustomProperty(alias, value));
            return this.Get(alias);
        }

        public IEnumerable<CustomProperty> AddOrUpdate(IDictionary<string, string> properties)
        {
            if (properties != null && properties.Any<KeyValuePair<string, string>>())
            {
                this.AddOrUpdate(
                    from p in properties
                    select new CustomProperty(p.Key, p.Value));
            }
            return this;
        }

        public void AddOrUpdate(IEnumerable<CustomProperty> properties)
        {
            if (properties == null)
            {
                return;
            }
            foreach (CustomProperty current in properties)
            {
                this.AddOrUpdate(current);
            }
        }

        public void AddOrUpdate(CustomProperty property)
        {
            if (property == null)
            {
                return;
            }
            CustomProperty customProperty = this.Get(property.Alias);
            if (customProperty != null)
            {
                if (!customProperty.IsReadOnly && (!customProperty.ServerSideOnly || customProperty.ServerSideOnly == property.ServerSideOnly))
                {
                    customProperty.IsReadOnly = property.IsReadOnly;
                    customProperty.ServerSideOnly = property.ServerSideOnly;
                    customProperty.Value = property.Value;
                    return;
                }
            }
            else
            {
                base.Add(property);
            }
        }

        public CustomPropertyCollection Copy()
        {
            CustomPropertyCollection customPropertyCollection = new CustomPropertyCollection();
            customPropertyCollection.AddRange(
                from kvp in this
                select kvp.Copy());
            return customPropertyCollection;
        }

        public override bool Equals(object obj)
        {
            CustomPropertyCollection customPropertyCollection = obj as CustomPropertyCollection;
            if (customPropertyCollection == null)
            {
                return false;
            }
            bool flag = base.Count == customPropertyCollection.Count;
            if (flag)
            {
                flag = !this.Any((CustomProperty p) => customPropertyCollection.All((CustomProperty cp) => !cp.Equals(p)));
            }
            return flag;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}