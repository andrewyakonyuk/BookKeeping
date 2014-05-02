using BookKeeping.Core;
using BookKeeping.Core.Domain;

namespace BookKeeping.Infrastructure
{
    public interface ICommandHandlerFactory
    {
        ICommandHandler<T> GetHandler<T>() where T : ICommand;
    }
}