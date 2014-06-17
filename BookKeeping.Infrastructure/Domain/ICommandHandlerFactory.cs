using BookKeeping.Domain.Contracts;

namespace BookKeeping.Domain
{
    public interface ICommandHandlerFactory
    {
        ICommandHandler<T> GetHandler<T>() where T : ICommand;
    }
}
