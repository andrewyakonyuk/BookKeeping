namespace BookKeeping.Domain.Models
{
    public class PaymentInformation : ICopyable<PaymentInformation>
    {
        public long CountryId
        {
            get;
            set;
        }

        public long? CountryRegionId
        {
            get;
            set;
        }

        public long? PaymentMethodId
        {
            get;
            set;
        }

        public string FirstName
        {
            get;
            set;
        }

        public string LastName
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public VatRate VatRate
        {
            get;
            set;
        }

        public PriceCollection TotalPrices
        {
            get;
            set;
        }

        public Price TotalPrice
        {
            get;
            set;
        }

        public PaymentInformation()
        {
            this.TotalPrices = new PriceCollection();
            this.TotalPrice = new Price();
        }

        public PaymentInformation(long countryId, long? countryRegionId = null)
            : this()
        {
            this.CountryId = countryId;
            this.CountryRegionId = countryRegionId;
        }

        public PaymentInformation Copy()
        {
            return new PaymentInformation
            {
                CountryId = this.CountryId,
                CountryRegionId = this.CountryRegionId,
                PaymentMethodId = this.PaymentMethodId,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Email = this.Email,
                VatRate = this.VatRate,
                TotalPrices = this.TotalPrices.Copy(),
                TotalPrice = this.TotalPrice.Copy()
            };
        }

        public override bool Equals(object obj)
        {
            PaymentInformation paymentInformation = obj as PaymentInformation;
            return paymentInformation != null && (this.CountryId == paymentInformation.CountryId && this.CountryRegionId == paymentInformation.CountryRegionId && this.PaymentMethodId == paymentInformation.PaymentMethodId && this.FirstName == paymentInformation.FirstName && this.LastName == paymentInformation.LastName && this.Email == paymentInformation.Email && this.VatRate.Equals(paymentInformation.VatRate) && this.TotalPrices.Equals(paymentInformation.TotalPrices)) && this.TotalPrice.Equals(paymentInformation.TotalPrice);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}