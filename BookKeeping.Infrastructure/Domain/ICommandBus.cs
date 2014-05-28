using BookKeeping.Domain.Contracts;
namespace BookKeeping.Infrastructure.Domain
{
    public interface ICommandBus
    {
        void Send<T>(T command) 
            where T : ICommand;
    }
}