namespace BookKeeping.Domain.Models
{
    public class PaymentMethodSetting
    {
        public long Id { get; set; }

        public long? LanguageId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public PaymentMethodSetting()
        {
        }

        public PaymentMethodSetting(string key, string value, long? languageId = null)
        {
            this.Key = key;
            this.Value = value;
            this.LanguageId = languageId;
        }
    }
}