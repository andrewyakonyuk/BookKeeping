using BookKeeping.Core;

namespace BookKeeping
{
    public class CommandBus : ICommandBus
    {
        private readonly ICommandHandlerFactory _commandHandlerFactory;

        public CommandBus(ICommandHandlerFactory commandHandlerFactory)
        {
            _commandHandlerFactory = commandHandlerFactory;
        }

        public void Send<T>(T command) where T : ICommand
        {
            var handler = _commandHandlerFactory.GetHandler<T>();
            if (handler != null)
            {
                handler.When(command);
            }
            else
            {
                throw new UnregisteredDomainCommandException("no handler registered");
            }
        }
    }
}