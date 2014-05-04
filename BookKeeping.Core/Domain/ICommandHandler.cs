namespace BookKeeping.Core.Domain
{
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        void When(TCommand c);
    }
}