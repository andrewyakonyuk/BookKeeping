namespace BookKeeping.Core
{
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        void When(TCommand command);
    }
}