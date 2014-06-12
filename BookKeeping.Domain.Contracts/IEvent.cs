namespace BookKeeping.Domain.Contracts
{
    public interface IEvent { }

    public interface IEvent<out TIdentity> : IEvent
        where TIdentity : IIdentity
    {
        TIdentity Id { get; }
    }
}