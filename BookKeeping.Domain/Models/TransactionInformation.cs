namespace BookKeeping.Domain.Models
{
    public class TransactionInformation : ICopyable<TransactionInformation>
    {
        public string TransactionId { get; set; }

        public PaymentState? PaymentState { get; set; }

        public string PaymentType { get; set; }

        public string PaymentIdentifier { get; set; }

        public PriceWithoutVat TransactionFee { get; set; }

        public PriceWithoutVat AmountAuthorized { get; set; }

        public TransactionInformation()
        {
            this.TransactionFee = new PriceWithoutVat();
            this.AmountAuthorized = new PriceWithoutVat();
        }

        public TransactionInformation Copy()
        {
            return new TransactionInformation()
            {
                TransactionId = this.TransactionId,
                PaymentState = this.PaymentState,
                PaymentType = this.PaymentType,
                PaymentIdentifier = this.PaymentIdentifier,
                TransactionFee = this.TransactionFee.Copy(),
                AmountAuthorized = this.TransactionFee.Copy()
            };
        }

        public override bool Equals(object obj)
        {
            TransactionInformation transactionInformation = obj as TransactionInformation;
            if (transactionInformation == null || !(this.TransactionId == transactionInformation.TransactionId))
                return false;
            PaymentState? paymentState1 = this.PaymentState;
            PaymentState? paymentState2 = transactionInformation.PaymentState;
            if ((paymentState1.GetValueOrDefault() != paymentState2.GetValueOrDefault() ? 0 : (paymentState1.HasValue == paymentState2.HasValue ? 1 : 0)) != 0 && this.PaymentType == transactionInformation.PaymentType && (this.PaymentIdentifier == transactionInformation.PaymentIdentifier && this.TransactionFee.Equals((object)transactionInformation.TransactionFee)))
                return this.AmountAuthorized.Equals((object)transactionInformation.AmountAuthorized);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}