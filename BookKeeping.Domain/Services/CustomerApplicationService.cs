using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Repositories;
using BookKeeping;
using BookKeeping.Domain;
using System;

namespace BookKeeping.Domain.Services
{
    /// <summary><para>
    /// This is an application service within the current bounded context.
    /// THis specific application service contains command handlers which load
    /// and operate a Customer aggregate. These handlers also pass required
    /// dependencies to aggregate methods and perform conflict resolution
    /// </para><para>
    /// Command handlers are usually invoked by an infrastructure of an application
    /// server, which hosts current service. Infrastructure will be responsible
    /// for accepting message calls (in form of web service calls or serialized
    ///  command messages) and dispatching them to these handlers.
    /// </para></summary>
    public sealed class CustomerApplicationService :
        ICustomerApplicationService,
        ICommandHandler<CreateCustomer>,
        ICommandHandler<RenameCustomer>,
        ICommandHandler<AddCustomerPayment>,
        ICommandHandler<ChargeCustomer>,
        ICommandHandler<LockCustomerForAccountOverdraft>,
        ICommandHandler<LockCustomer>,
        ICommandHandler<DeleteCustomer>,
        ICommandHandler<UpdateCustomerAddress>,
        ICommandHandler<UpdateCustomerInfo>
    {
        // domain service that is neeeded by aggregate
        private readonly IPricingService _pricingService;

        readonly IRepository<Customer, CustomerId> _repository;

         public CustomerApplicationService(IRepository<Customer, CustomerId> repository, IPricingService pricingService)
        {
            _repository = repository;
            _pricingService = pricingService;
        }

         private void Update(CustomerId id, Action<Customer> execute)
        {
            var customer = _repository.Get(id);
            execute(customer);
        }

        public void LockCustomerForAccountOverdraft(CustomerId customerId, string comment)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            var customer = _repository.Get(customerId);
            customer.LockForAccountOverdraft(comment, _pricingService,Current.Identity.Id, Current.UtcNow);
        }

        public void When(CreateCustomer c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, a => a.Create(c.Id, c.Name, c.Currency, _pricingService, Current.Identity.Id, Current.UtcNow));
        }

        public void When(RenameCustomer c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, a => a.Rename(c.NewName, Current.Identity.Id, Current.UtcNow));
        }

        public void When(AddCustomerPayment c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, a => a.AddPayment(c.Name, c.Amount, Current.Identity.Id, Current.UtcNow));
        }

        public void When(ChargeCustomer c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, a => a.Charge(c.Name, c.Amount, Current.Identity.Id, Current.UtcNow));
        }

        public void When(LockCustomerForAccountOverdraft c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, a => a.LockForAccountOverdraft(c.Comment, _pricingService, Current.Identity.Id, Current.UtcNow));
        }

        public void When(LockCustomer c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, a => a.LockCustomer(c.Reason, Current.Identity.Id, Current.UtcNow));
        }

        public void When(DeleteCustomer c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, a => a.Delete(Current.Identity.Id, Current.UtcNow));
        }

        public void When(UpdateCustomerAddress c)
        {
            Update(c.Id, a => a.UpdateAddress(c.Address, Current.Identity.Id, Current.UtcNow));
        }

        public void When(UpdateCustomerInfo c)
        {
            Update(c.Id, a => a.UpdateInfo(c.BankingDetails, c.Phone, c.Fax, c.Email, Current.Identity.Id, Current.UtcNow));
        }
    }
}