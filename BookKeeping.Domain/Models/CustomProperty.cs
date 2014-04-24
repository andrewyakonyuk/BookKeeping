namespace BookKeeping.Domain.Models
{
    public class CustomProperty : ICopyable<CustomProperty>
    {
        public string Alias { get; set; }

        public string Value { get; set; }

        public bool ServerSideOnly { get; set; }

        public bool IsReadOnly { get; set; }

        public CustomProperty()
        {
        }

        public CustomProperty(string alias, string value)
            : this()
        {
            this.Alias = alias;
            this.Value = value;
        }

        public CustomProperty Copy()
        {
            return new CustomProperty()
            {
                Alias = this.Alias,
                Value = this.Value,
                ServerSideOnly = this.ServerSideOnly,
                IsReadOnly = this.IsReadOnly
            };
        }

        public override string ToString()
        {
            return this.Value;
        }

        public override bool Equals(object obj)
        {
            CustomProperty customProperty = obj as CustomProperty;
            if (customProperty == null || !(this.Alias == customProperty.Alias) || (!(this.Value == customProperty.Value) || this.ServerSideOnly != customProperty.ServerSideOnly))
                return false;
            else
                return this.IsReadOnly == customProperty.IsReadOnly;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}