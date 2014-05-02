namespace BookKeeping.Core.Domain
{
    public interface IEventHandler<TEvent> 
        where TEvent : IEvent
    {
        void When(TEvent e);
    }
}