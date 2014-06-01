using BookKeeping.Domain.Contracts;

namespace BookKeeping.Infrastructure.Domain
{
    public interface ICommandHandlerFactory
    {
        ICommandHandler<T> GetHandler<T>() where T : ICommand;
    }
}
