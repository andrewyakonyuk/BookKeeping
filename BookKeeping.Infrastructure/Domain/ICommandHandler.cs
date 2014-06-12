using BookKeeping.Domain.Contracts;
namespace BookKeeping.Infrastructure.Domain
{
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        void When(TCommand c);
    }
}