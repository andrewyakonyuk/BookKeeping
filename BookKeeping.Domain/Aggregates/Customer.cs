using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Services;
using BookKeeping.Domain;
using System;
using System.Collections.Generic;

namespace BookKeeping.Domain.Aggregates
{
    /// <summary>
    /// <para>Implementation of customer aggregate. In production it is loaded and
    /// operated by an <see cref="CustomerApplicationService"/>, which loads it from
    /// the event storage and calls appropriate methods, passing needed arguments in.</para>
    /// <para>In test environments (e.g. in unit tests), this aggregate can be
    /// instantiated directly.</para>
    /// </summary>
    public class Customer : AggregateBase, ICustomerState
    {
        public string Name { get; private set; }

        public bool Created { get; private set; }

        public CustomerId Id { get; private set; }

        public bool ConsumptionLocked { get; private set; }

        public bool ManualBilling { get; private set; }

        public Currency Currency { get; private set; }

        public CurrencyAmount Balance { get; private set; }

        public int MaxTransactionId { get; private set; }

        public Address LegalAddress { get; private set; }

        public string BankingDetails { get; private set; }

        public string Phone { get; private set; }

        public string Fax { get; private set; }

        public string Email { get; private set; }

        public Customer(IEnumerable<IEvent> events)
            : base(events)
        {
            LegalAddress = new Address();
        }

        public void Create(CustomerId id, string name, Currency currency, IPricingService service, UserId userId, DateTime utc)
        {
            if (this.Created)
                throw new InvalidOperationException("Customer was already created");
            Apply(new CustomerCreated(id, name, currency, userId, utc));

            var bonus = service.GetWelcomeBonus(currency);
            if (bonus.Amount > 0)
                AddPayment("Welcome bonus", bonus, userId, utc);
        }

        public void Rename(string name, UserId userId, DateTime utc)
        {
            if (this.Name == name)
                return;
            Apply(new CustomerRenamed(this.Id, name, this.Name, userId, utc));
        }

        public void LockCustomer(string reason, UserId userId, DateTime utc)
        {
            if (this.ConsumptionLocked)
                return;

            Apply(new CustomerLocked(this.Id, reason, userId, utc));
        }

        public void LockForAccountOverdraft(string comment, IPricingService service, UserId userId, DateTime utc)
        {
            if (this.ManualBilling) return;
            var threshold = service.GetOverdraftThreshold(this.Currency);
            if (this.Balance < threshold)
            {
                LockCustomer("Overdraft. " + comment, userId, utc);
            }
        }

        public void AddPayment(string name, CurrencyAmount amount, UserId userId, DateTime utc)
        {
            Apply(new CustomerPaymentAdded(this.Id, name, amount, this.Balance + amount, this.MaxTransactionId + 1, userId, utc));
        }

        public void Charge(string name, CurrencyAmount amount, UserId userId, DateTime utc)
        {
            if (this.Currency == Currency.Undefined)
                throw new InvalidOperationException("Customer currency was not assigned!");
            Apply(new CustomerChargeAdded(this.Id, name, amount, this.Balance - amount, this.MaxTransactionId + 1, userId, utc));
        }

        public void Delete(UserId userId, DateTime utc)
        {
            Apply(new CustomerDeleted(this.Id, userId, utc));
        }

        public void UpdateAddress(Address address, UserId userId, DateTime utc)
        {
            Apply(new CustomerAddressUpdated(this.Id, address, userId, utc));
        }

        public void UpdateInfo(string bankingDetails, string phone, string fax, string email, UserId userId, DateTime utc)
        {
            Apply(new CustomerInfoUpdated(this.Id, bankingDetails, phone, fax, email, userId, utc));
        }

        protected override void Mutate(IEvent e)
        {
            Version += 1;
            ((ICustomerState)this).When((dynamic)e);
        }

        void ICustomerState.When(CustomerLocked e)
        {
            ConsumptionLocked = true;
        }

        void ICustomerState.When(CustomerPaymentAdded e)
        {
            Balance = e.NewBalance;
            MaxTransactionId = e.Transaction;
        }

        void ICustomerState.When(CustomerChargeAdded e)
        {
            Balance = e.NewBalance;
            MaxTransactionId = e.Transaction;
        }

        void ICustomerState.When(CustomerCreated e)
        {
            Created = true;
            Name = e.Name;
            Id = e.Id;
            Currency = e.Currency;
            Balance = new CurrencyAmount(0, e.Currency);
        }

        void ICustomerState.When(CustomerRenamed e)
        {
            Name = e.Name;
        }

        void ICustomerState.When(CustomerDeleted e)
        {
            this.Version = -1;
        }

        void ICustomerState.When(CustomerAddressUpdated e)
        {
            LegalAddress = e.Address;
        }

        void ICustomerState.When(CustomerInfoUpdated e)
        {
            BankingDetails = e.BankingDetails;
            Fax = e.Fax;
            Email = e.Email;
            Phone = e.Phone;
        }
    }
}