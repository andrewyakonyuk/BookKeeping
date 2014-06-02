using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Services;
using BookKeeping.Infrastructure.Domain;
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
    public class Customer : AggregateBase
    {
        public string Name { get; private set; }

        public bool Created { get; private set; }

        public CustomerId Id { get; private set; }

        public bool ConsumptionLocked { get; private set; }

        public bool ManualBilling { get; private set; }

        public Currency Currency { get; private set; }

        public CurrencyAmount Balance { get; private set; }

        public int MaxTransactionId { get; private set; }

        public Customer(IEnumerable<IEvent> events)
            : base(events)
        {
        }

        public void Create(CustomerId id, string name, Currency currency, IPricingService service, DateTime utc)
        {
            if (this.Created)
                throw new InvalidOperationException("Customer was already created");
            Apply(new CustomerCreated
            {
                Created = utc,
                Name = name,
                Id = id,
                Currency = currency
            });

            var bonus = service.GetWelcomeBonus(currency);
            if (bonus.Amount > 0)
                AddPayment("Welcome bonus", bonus, utc);
        }

        public void Rename(string name, DateTime dateTime)
        {
            if (this.Name == name)
                return;
            Apply(new CustomerRenamed
            {
                Name = name,
                Id = this.Id,
                OldName = this.Name,
                Renamed = dateTime
            });
        }

        public void LockCustomer(string reason)
        {
            if (this.ConsumptionLocked)
                return;

            Apply(new CustomerLocked
            {
                Id = this.Id,
                Reason = reason
            });
        }

        public void LockForAccountOverdraft(string comment, IPricingService service)
        {
            if (this.ManualBilling) return;
            var threshold = service.GetOverdraftThreshold(this.Currency);
            if (this.Balance < threshold)
            {
                LockCustomer("Overdraft. " + comment);
            }
        }

        public void AddPayment(string name, CurrencyAmount amount, DateTime utc)
        {
            Apply(new CustomerPaymentAdded()
            {
                Id = this.Id,
                Payment = amount,
                NewBalance = this.Balance + amount,
                PaymentName = name,
                Transaction = this.MaxTransactionId + 1,
                TimeUtc = utc
            });
        }

        public void Charge(string name, CurrencyAmount amount, DateTime time)
        {
            if (this.Currency == Currency.Undefined)
                throw new InvalidOperationException("Customer currency was not assigned!");
            Apply(new CustomerChargeAdded()
            {
                Id = this.Id,
                Charge = amount,
                NewBalance = this.Balance - amount,
                ChargeName = name,
                Transaction = this.MaxTransactionId + 1,
                TimeUtc = time
            });
        }

        public void When(CustomerLocked e)
        {
            ConsumptionLocked = true;
        }

        public void When(CustomerPaymentAdded e)
        {
            Balance = e.NewBalance;
            MaxTransactionId = e.Transaction;
        }

        public void When(CustomerChargeAdded e)
        {
            Balance = e.NewBalance;
            MaxTransactionId = e.Transaction;
        }

        public void When(CustomerCreated e)
        {
            Created = true;
            Name = e.Name;
            Id = e.Id;
            Currency = e.Currency;
            Balance = new CurrencyAmount(0, e.Currency);
        }

        public void When(CustomerRenamed e)
        {
            Name = e.Name;
        }
    }
}