namespace BookKeeping.Core.Domain
{
    public interface IEvent { }

    public interface IEvent<out TIdentity> : IEvent
        where TIdentity : IIdentity
    {
        TIdentity Id { get; }
    }
}