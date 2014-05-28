using BookKeeping.Domain.Contracts;
namespace BookKeeping.Infrastructure.Domain
{
    public interface IEventBus : IUnitOfWork
    {
        void Publish<T>(T @event) 
            where T : IEvent;
    }
}