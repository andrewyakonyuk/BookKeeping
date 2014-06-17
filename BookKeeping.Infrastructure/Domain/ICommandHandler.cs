using BookKeeping.Domain.Contracts;
namespace BookKeeping.Domain
{
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        void When(TCommand c);
    }
}