using BookKeeping.Persistance.AtomicStorage;
using System.Runtime.Serialization;

namespace BookKeeping.Domain
{
    public sealed class DomainIdentityGenerator : IDomainIdentityGenerator
    {
        readonly IDocumentStore _storage;

        public DomainIdentityGenerator(IDocumentStore storage)
        {
            _storage = storage;
        }

        public long GetId()
        {
            var ix = new long[1];
            _storage.GetWriter<unit, DomainIdentityVector>().UpdateEnforcingNew(unit.it, t => t.Reserve(ix));
            return ix[0];
        }

        public void IncrementDomainIdentity(long id)
        {
            _storage.GetWriter<unit, DomainIdentityVector>().UpdateEnforcingNew(unit.it, t =>
            {
                if (t.EntityId < id)
                {
                    t.EntityId = id;
                }
            });
        }
    }

    [DataContract(Namespace = "hub-domain-data", Name = "domainidentityvector")]
    public sealed class DomainIdentityVector
    {
        [DataMember(Order = 1)]
        public long EntityId { get; set; }

        public void Reserve(long[] indexes)
        {
            for (int i = 0; i < indexes.Length; i++)
            {
                EntityId += 1;
                indexes[i] = EntityId;
            }
        }
    }
}
