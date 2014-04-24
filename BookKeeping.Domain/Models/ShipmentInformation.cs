namespace BookKeeping.Domain.Models
{
    public class ShipmentInformation : ICopyable<ShipmentInformation>
    {
        public long? CountryId { get; set; }

        public long? CountryRegionId { get; set; }

        public long? ShippingMethodId { get; set; }

        public VatRate VatRate { get; set; }

        public PriceCollection TotalPrices { get; set; }

        public Price TotalPrice { get; set; }

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
            return new ShipmentInformation()
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
            if (shipmentInformation == null)
                return false;
            long? countryId1 = this.CountryId;
            long? countryId2 = shipmentInformation.CountryId;
            if ((countryId1.GetValueOrDefault() != countryId2.GetValueOrDefault() ? 0 : (countryId1.HasValue == countryId2.HasValue ? 1 : 0)) != 0)
            {
                long? countryRegionId1 = this.CountryRegionId;
                long? countryRegionId2 = shipmentInformation.CountryRegionId;
                if ((countryRegionId1.GetValueOrDefault() != countryRegionId2.GetValueOrDefault() ? 0 : (countryRegionId1.HasValue == countryRegionId2.HasValue ? 1 : 0)) != 0)
                {
                    long? shippingMethodId1 = this.ShippingMethodId;
                    long? shippingMethodId2 = shipmentInformation.ShippingMethodId;
                    if ((shippingMethodId1.GetValueOrDefault() != shippingMethodId2.GetValueOrDefault() ? 0 : (shippingMethodId1.HasValue == shippingMethodId2.HasValue ? 1 : 0)) != 0 && this.VatRate.Equals((object)shipmentInformation.VatRate) && this.TotalPrices.Equals((object)shipmentInformation.TotalPrices))
                        return this.TotalPrice.Equals((object)shipmentInformation.TotalPrice);
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}