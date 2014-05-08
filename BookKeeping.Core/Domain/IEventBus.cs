namespace BookKeeping.Core.Domain
{
    public interface IEventBus : IUnitOfWork
    {
        void Publish<T>(T @event) 
            where T : IEvent;
    }
}