using BookKeeping.Domain;
using BookKeeping.Domain.Contracts;

namespace BookKeeping.Domain
{
    public interface ISession : IUnitOfWork, IDomainIdentityGenerator
    {
        void Command<TCommand>(TCommand command)
         where TCommand : ICommand;

        Maybe<TView> Query<TKey, TView>(TKey id);

        Maybe<TView> Query<TView>();
    }
}
