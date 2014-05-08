namespace BookKeeping.Core.Domain
{
    public interface ICommandBus : IUnitOfWork
    {
        void Send<T>(T command) 
            where T : ICommand;
    }
}