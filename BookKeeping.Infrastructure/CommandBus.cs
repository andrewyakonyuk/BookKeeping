using BookKeeping.Core;
using BookKeeping.Core.Domain;
using System;
using System.Collections.Generic;

namespace BookKeeping.Infrastructure
{
    public class CommandBus : ICommandBus
    {
        private readonly ICommandHandlerFactory _commandHandlerFactory;
        private readonly Queue<Action> _queue = new Queue<Action>();

        public CommandBus(ICommandHandlerFactory commandHandlerFactory)
        {
            _commandHandlerFactory = commandHandlerFactory;
        }

        public void Send<T>(T command) where T : ICommand
        {
            var handler = _commandHandlerFactory.GetHandler<T>();
            if (handler != null)
            {
                _queue.Enqueue(() => handler.When(command));
            }
            else
            {
                throw new UnregisteredDomainCommandException("no handler registered");
            }
        }

        public void Commit()
        {
            while (_queue.Count > 0)
            {
                _queue.Dequeue()();
            }
        }

        public void Rollback()
        {
            _queue.Clear();
        }
    }
}