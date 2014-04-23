using System;

namespace BookKeeping.Domain.CustomerAggregate
{

    public class Customer
    {
        public string Name { get; protected set; }

        public bool Created { get; protected set; }

        public CustomerId Id { get; protected set; }

        public bool ConsumptionLocked { get; protected set; }

        public bool ManualBilling { get; protected set; }

        public Currency Currency { get; protected set; }

        public CurrencyAmount Balance { get; protected set; }

        public int MaxTransactionId { get; protected set; }

        [Obsolete("Only for NHibernate", true)]
        protected Customer()
        {

        }

        public Customer(CustomerId id, string name, Currency currency, IPricingService service)
        {
            Name = name;
            Currency = currency;
            Id = id;
            Created = true;

            var bonus = service.GetWelcomeBonus(currency);
            if (bonus.Amount > 0)
                AddPayment("Welcome bonus", bonus, DateTime.UtcNow);
        }

        public void Rename(string name, DateTime dateTime)
        {
            if (string.Equals(Name, name, StringComparison.CurrentCulture))
                return;
            Name = name;
        }

        public void LockCustomer(string reason)
        {
            if (ConsumptionLocked)
                return;

            ConsumptionLocked = true;
        }

        public void LockForAccountOverdraft(string comment, IPricingService service)
        {
            if (ManualBilling) return;
            var threshold = service.GetOverdraftThreshold(Currency);
            if (Balance < threshold)
            {
                LockCustomer("Overdraft. " + comment);
            }
        }

        public void AddPayment(string name, CurrencyAmount amount, DateTime utc)
        {
            if (Currency == Currency.None)
                throw new InvalidOperationException("Customer currency was not assigned!");

            Balance = Balance + amount;
            MaxTransactionId = MaxTransactionId + 1;
        }

        public void Charge(string name, CurrencyAmount amount, DateTime time)
        {
            if (Currency == Currency.None)
                throw new InvalidOperationException("Customer currency was not assigned!");

            Balance = Balance - amount;
            MaxTransactionId = MaxTransactionId + 1;
        }
    }
}