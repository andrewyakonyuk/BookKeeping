using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Repositories;
using BookKeeping;
using BookKeeping.Domain;
using System;

namespace BookKeeping.Domain.Services
{
    public sealed class VendorApplicationService :
        IVendorApplicationService,
        ICommandHandler<CreateVendor>,
        ICommandHandler<RenameVendor>,
        ICommandHandler<AddVendorPayment>,
        ICommandHandler<ChargeVendor>,
        ICommandHandler<LockVendorForAccountOverdraft>,
        ICommandHandler<LockVendor>,
        ICommandHandler<DeleteVendor>,
        ICommandHandler<UpdateVendorAddress>,
        ICommandHandler<UpdateVendorInfo>
    {
        // domain service that is neeeded by aggregate
        private readonly IPricingService _pricingService;

        readonly IRepository<Vendor, VendorId> _repository;

         public VendorApplicationService(IRepository<Vendor, VendorId> repository, IPricingService pricingService)
        {
            _repository = repository;
            _pricingService = pricingService;
        }

         private void Update(VendorId id, Action<Vendor> execute)
        {
            var vendor = _repository.Get(id);
            execute(vendor);
        }

        public void LockVendorForAccountOverdraft(VendorId customerId, string comment)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            var customer = _repository.Get(customerId);
            customer.LockForAccountOverdraft(comment, _pricingService,Current.Identity.Id, Current.UtcNow);
        }

        public void When(CreateVendor c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, a => a.Create(c.Id, c.Name, c.Currency, _pricingService, Current.Identity.Id, Current.UtcNow));
        }

        public void When(RenameVendor c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, a => a.Rename(c.NewName, Current.Identity.Id, Current.UtcNow));
        }

        public void When(AddVendorPayment c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, a => a.AddPayment(c.Name, c.Amount, Current.Identity.Id, Current.UtcNow));
        }

        public void When(ChargeVendor c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, a => a.Charge(c.Name, c.Amount, Current.Identity.Id, Current.UtcNow));
        }

        public void When(LockVendorForAccountOverdraft c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, a => a.LockForAccountOverdraft(c.Comment, _pricingService, Current.Identity.Id, Current.UtcNow));
        }

        public void When(LockVendor c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, a => a.LockVendor(c.Reason, Current.Identity.Id, Current.UtcNow));
        }

        public void When(DeleteVendor c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, a => a.Delete(Current.Identity.Id, Current.UtcNow));
        }

        public void When(UpdateVendorAddress c)
        {
            Update(c.Id, a => a.UpdateAddress(c.Address, Current.Identity.Id, Current.UtcNow));
        }

        public void When(UpdateVendorInfo c)
        {
            Update(c.Id, a => a.UpdateInfo(c.BankingDetails, c.Phone, c.Fax, c.Email, Current.Identity.Id, Current.UtcNow));
        }
    }
}