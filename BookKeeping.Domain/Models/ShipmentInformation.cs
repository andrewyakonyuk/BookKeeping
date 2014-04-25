namespace BookKeeping.Domain.Models
{
    public class ShipmentInformation : ICopyable<ShipmentInformation>
    {
        public long? CountryId
        {
            get;
            set;
        }

        public long? CountryRegionId
        {
            get;
            set;
        }

        public long? ShippingMethodId
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

        public ShipmentInformation()
        {
            this.TotalPrices = new PriceCollection();
            this.TotalPrice = new Price();
        }

        public ShipmentInformation(long countryId, long? countryRegionId = null)
            : this()
        {
            this.CountryId = new long?(countryId);
            this.CountryRegionId = countryRegionId;
        }

        public ShipmentInformation Copy()
        {
            return new ShipmentInformation
            {
                CountryId = this.CountryId,
                CountryRegionId = this.CountryRegionId,
                ShippingMethodId = this.ShippingMethodId,
                VatRate = this.VatRate,
                TotalPrices = this.TotalPrices.Copy(),
                TotalPrice = this.TotalPrice.Copy()
            };
        }

        public override bool Equals(object obj)
        {
            ShipmentInformation shipmentInformation = obj as ShipmentInformation;
            return shipmentInformation != null && (this.CountryId == shipmentInformation.CountryId && this.CountryRegionId == shipmentInformation.CountryRegionId && this.ShippingMethodId == shipmentInformation.ShippingMethodId && this.VatRate.Equals(shipmentInformation.VatRate) && this.TotalPrices.Equals(shipmentInformation.TotalPrices)) && this.TotalPrice.Equals(shipmentInformation.TotalPrice);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}