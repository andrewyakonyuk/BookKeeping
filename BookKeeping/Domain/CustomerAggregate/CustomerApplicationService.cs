using BookKeeping.Core;
using BookKeeping.Core.Storage;
using System;

namespace BookKeeping.Domain.CustomerAggregate
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
    public sealed class CustomerApplicationService : ICommandHandler<CreateCustomer>,
        ICommandHandler<RenameCustomer>,
        ICommandHandler<AddCustomerPayment>,
        ICommandHandler<ChargeCustomer>,
        ICommandHandler<LockCustomerForAccountOverdraft>,
        ICommandHandler<LockCustomer>
    {
        private readonly IEventBus _eventBus;

        // event store for accessing event streams
        private readonly IEventStore _eventStore;

        // domain service that is neeeded by aggregate
        private readonly IPricingService _pricingService;

        // pass dependencies for this application service via constructor
        public CustomerApplicationService(IEventStore eventStore, IEventBus eventBus, IPricingService pricingService)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
            _pricingService = pricingService;
        }

        public void LockCustomerForAccountOverdraft(CustomerId customerId, string comment)
        {
            // Step 2.1: Load event stream for Customer, given its id
            var stream = _eventStore.LoadEventStream(customerId);
            // Step 2.2: Build aggregate from event stream
            var customer = new Customer(stream.Events);
            // Step 3: call aggregate method, passing it arguments and pricing domain service
            customer.LockForAccountOverdraft(comment, _pricingService);
            // Step 4: commit changes to the event stream by id
            _eventStore.AppendToStream(customerId, stream.Version, customer.Changes);
        }

        public void When(CreateCustomer c)
        {
            Update(c.Id, a => a.Create(c.Id, c.Name, c.Currency, _pricingService, DateTime.UtcNow));
        }

        public void When(RenameCustomer c)
        {
            Update(c.Id, a => a.Rename(c.NewName, DateTime.UtcNow));
        }

        public void When(AddCustomerPayment c)
        {
            Update(c.Id, a => a.AddPayment(c.Name, c.Amount, DateTime.UtcNow));
        }

        public void When(ChargeCustomer c)
        {
            Update(c.Id, a => a.Charge(c.Name, c.Amount, DateTime.UtcNow));
        }

        public void When(LockCustomerForAccountOverdraft c)
        {
            Update(c.Id, a => a.LockForAccountOverdraft(c.Comment, _pricingService));
        }

        public void When(LockCustomer c)
        {
            Update(c.Id, a => a.LockCustomer(c.Reason));
        }

        private static bool ConflictsWith(IEvent x, IEvent y)
        {
            return x.GetType() == y.GetType();
        }

        private void Update(CustomerId id, Action<Customer> execute)
        {
            // Load event stream from the store
            EventStream stream = _eventStore.LoadEventStream(id);
            // create new Customer aggregate from the history
            Customer customer = new Customer(stream.Events);
            // execute delegated action
            execute(customer);
            // append resulting changes to the stream
            _eventStore.AppendToStream(id, stream.Version, customer.Changes);

            foreach (var @event in customer.Changes)
            {
                var realEvent = System.Convert.ChangeType(@event, @event.GetType());
                _eventBus.Publish(@event);
            }
        }

        // Sample of method that would apply simple conflict resolution.
        // see IDDD book or Greg Young's videos for more in-depth explanation
        private void UpdateWithSimpleConflictResolution(CustomerId id, Action<Customer> execute)
        {
            while (true)
            {
                EventStream eventStream = _eventStore.LoadEventStream(id);
                Customer customer = new Customer(eventStream.Events);
                execute(customer);

                try
                {
                    _eventStore.AppendToStream(id, eventStream.Version, customer.Changes);

                    foreach (var @event in customer.Changes)
                    {
                        var realEvent = System.Convert.ChangeType(@event, @event.GetType());
                        _eventBus.Publish(@event);
                    }
                    return;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    foreach (var clientEvent in customer.Changes)
                    {
                        foreach (var actualEvent in ex.ActualEvents)
                        {
                            if (ConflictsWith(clientEvent, actualEvent))
                            {
                                var msg = string.Format("Conflict between {0} and {1}",
                                    clientEvent, actualEvent);
                                throw new RealConcurrencyException(msg, ex);
                            }
                        }
                    }
                    // there are no conflicts and we can append
                    _eventStore.AppendToStream(id, ex.ActualVersion, customer.Changes);

                    foreach (var @event in customer.Changes)
                    {
                        var realEvent = System.Convert.ChangeType(@event, @event.GetType());
                        _eventBus.Publish(@event);
                    }
                }
            }
        }
    }
}