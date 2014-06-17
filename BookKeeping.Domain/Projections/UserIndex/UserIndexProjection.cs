using BookKeeping.Domain.Contracts;
using BookKeeping.Domain;
using BookKeeping.Persistance;
using BookKeeping.Persistance.AtomicStorage;
using System.Linq;

namespace BookKeeping.Domain.Projections.UserIndex
{
    public sealed class UserIndexProjection:
        IEventHandler<UserCreated>,
        IEventHandler<UserDeleted>
    {
        readonly IDocumentWriter<unit, UserIndexLookup> _writer;

        public UserIndexProjection(IDocumentWriter<unit, UserIndexLookup> writer)
        {
            _writer = writer;
        }

        public void When(UserCreated e)
        {
            _writer.UpdateEnforcingNew(unit.it, si =>
            {
                si.Logins[e.Login] = e.Id;
                si.Identities.Add(e.Id);
            });
        }

        public void When(UserDeleted e)
        {
            _writer.UpdateEnforcingNew(unit.it, si =>
            {
                si.Logins.Remove(si.Logins.SingleOrDefault(t => t.Value == e.Id));
                si.Identities.Remove(e.Id);
            });
        }
    }
}
