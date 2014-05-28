using BookKeeping.Domain.Contracts;
namespace BookKeeping.Infrastructure.Domain
{
    public interface IEventHandler<TEvent> 
        where TEvent : IEvent
    {
        void When(TEvent e);
    }
}