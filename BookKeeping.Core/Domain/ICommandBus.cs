namespace BookKeeping.Core.Domain
{
    public interface ICommandBus
    {
        void Send<T>(T command) 
            where T : ICommand;
    }
}