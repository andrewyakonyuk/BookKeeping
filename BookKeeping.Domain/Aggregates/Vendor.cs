using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Services;
using BookKeeping.Infrastructure.Domain;
using System;
using System.Collections.Generic;

namespace BookKeeping.Domain.Aggregates
{
    public class Vendor : AggregateBase, IVendorState
    {
        public string Name { get; private set; }

        public bool Created { get; private set; }

        public VendorId Id { get; private set; }

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

        public Vendor(IEnumerable<IEvent> events)
            : base(events)
        {
            LegalAddress = new Address();
        }

        public void Create(VendorId id, string name, Currency currency, IPricingService service, UserId userId, DateTime utc)
        {
            if (this.Created)
                throw new InvalidOperationException("Vendor was already created");
            Apply(new VendorCreated(id, name, currency, userId, utc));

            var bonus = service.GetWelcomeBonus(currency);
            if (bonus.Amount > 0)
                AddPayment("Welcome bonus", bonus, userId, utc);
        }

        public void Rename(string name, UserId userId, DateTime utc)
        {
            if (this.Name == name)
                return;
            Apply(new VendorRenamed(this.Id, name, this.Name, userId, utc));
        }

        public void LockVendor(string reason, UserId userId, DateTime utc)
        {
            if (this.ConsumptionLocked)
                return;

            Apply(new VendorLocked(this.Id, reason, userId, utc));
        }

        public void LockForAccountOverdraft(string comment, IPricingService service, UserId userId, DateTime utc)
        {
            if (this.ManualBilling) return;
            var threshold = service.GetOverdraftThreshold(this.Currency);
            if (this.Balance < threshold)
            {
                LockVendor("Overdraft. " + comment, userId, utc);
            }
        }

        public void AddPayment(string name, CurrencyAmount amount, UserId userId, DateTime utc)
        {
            Apply(new VendorPaymentAdded(this.Id, name, amount, this.Balance + amount, this.MaxTransactionId + 1, userId, utc));
        }

        public void Charge(string name, CurrencyAmount amount, UserId userId, DateTime utc)
        {
            if (this.Currency == Currency.Undefined)
                throw new InvalidOperationException("Vendor currency was not assigned!");
            Apply(new VendorChargeAdded(this.Id, name, amount, this.Balance - amount, this.MaxTransactionId + 1, userId, utc));
        }

        public void Delete(UserId userId, DateTime utc)
        {
            Apply(new VendorDeleted(this.Id, userId, utc));
        }

        public void UpdateAddress(Address address, UserId userId, DateTime utc)
        {
            Apply(new VendorAddressUpdated(this.Id, address, userId, utc));
        }

        public void UpdateInfo(string bankingDetails, string phone, string fax, string email, UserId userId, DateTime utc)
        {
            Apply(new VendorInfoUpdated(this.Id, bankingDetails, phone, fax, email, userId, utc));
        }

        protected override void Mutate(IEvent e)
        {
            Version += 1;
            ((IVendorState)this).When((dynamic)e);
        }

        void IVendorState.When(VendorLocked e)
        {
            ConsumptionLocked = true;
        }

        void IVendorState.When(VendorPaymentAdded e)
        {
            Balance = e.NewBalance;
            MaxTransactionId = e.Transaction;
        }

        void IVendorState.When(VendorChargeAdded e)
        {
            Balance = e.NewBalance;
            MaxTransactionId = e.Transaction;
        }

        void IVendorState.When(VendorCreated e)
        {
            Created = true;
            Name = e.Name;
            Id = e.Id;
            Currency = e.Currency;
            Balance = new CurrencyAmount(0, e.Currency);
        }

        void IVendorState.When(VendorRenamed e)
        {
            Name = e.Name;
        }

        void IVendorState.When(VendorDeleted e)
        {
            this.Version = -1;
        }

        void IVendorState.When(VendorAddressUpdated e)
        {
            LegalAddress = e.Address;
        }

        void IVendorState.When(VendorInfoUpdated e)
        {
            BankingDetails = e.BankingDetails;
            Fax = e.Fax;
            Email = e.Email;
            Phone = e.Phone;
        }
    }
}