namespace BookKeeping.Core.Domain
{
    public interface IEventBus
    {
        void Publish<T>(T @event) 
            where T : IEvent;
    }
}