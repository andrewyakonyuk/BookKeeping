using BookKeeping.Domain.Contracts;
namespace BookKeeping.Domain
{
    public interface IEventBus
    {
        void Publish<T>(T @event) 
            where T : IEvent;
    }
}