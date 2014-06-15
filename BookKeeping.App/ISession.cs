using BookKeeping.Domain;
using BookKeeping.Domain.Contracts;
using BookKeeping.Infrastructure.Domain;
using BookKeeping.Persistent;

namespace BookKeeping.App
{
    public interface ISession : IUnitOfWork, IDomainIdentityService
    {
        void Command<TCommand>(TCommand command)
         where TCommand : ICommand;

        Maybe<TView> Query<TKey, TView>(TKey id);

        Maybe<TView> Query<TView>();
    }
}
