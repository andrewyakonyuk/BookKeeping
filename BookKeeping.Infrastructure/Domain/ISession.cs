using BookKeeping.Domain;
using BookKeeping.Domain.Contracts;

namespace BookKeeping.Domain
{
    public interface ISession : IUnitOfWork, IDomainIdentityGenerator
    {
        void Command<TCommand>(TCommand command)
         where TCommand : ICommand;

        IQueryFor<TResult> Query<TResult>();
    }
}
