namespace BookKeeping.Core
{
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        void When(TEvent e);
    }
}