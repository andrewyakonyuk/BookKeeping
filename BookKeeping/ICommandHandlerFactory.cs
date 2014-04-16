using BookKeeping.Core;

namespace BookKeeping
{
    public interface ICommandHandlerFactory
    {
        ICommandHandler<T> GetHandler<T>() where T : ICommand;
    }
}