namespace BookKeeping.Domain.Models
{
    public class TransactionInformation : ICopyable<TransactionInformation>
    {
        public string TransactionId
        {
            get;
            set;
        }

        public PaymentState? PaymentState
        {
            get;
            set;
        }

        public string PaymentType
        {
            get;
            set;
        }

        public string PaymentIdentifier
        {
            get;
            set;
        }

        public PriceWithoutVat TransactionFee
        {
            get;
            set;
        }

        public PriceWithoutVat AmountAuthorized
        {
            get;
            set;
        }

        public TransactionInformation()
        {
            this.TransactionFee = new PriceWithoutVat();
            this.AmountAuthorized = new PriceWithoutVat();
        }

        public TransactionInformation Copy()
        {
            return new TransactionInformation
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
            return transactionInformation != null && (this.TransactionId == transactionInformation.TransactionId && this.PaymentState == transactionInformation.PaymentState && this.PaymentType == transactionInformation.PaymentType && this.PaymentIdentifier == transactionInformation.PaymentIdentifier && this.TransactionFee.Equals(transactionInformation.TransactionFee)) && this.AmountAuthorized.Equals(transactionInformation.AmountAuthorized);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}