using BookKeeping.Domain.Contracts;
namespace BookKeeping.Domain
{
    public interface ICommandBus
    {
        void Send<T>(T command) 
            where T : ICommand;
    }
}