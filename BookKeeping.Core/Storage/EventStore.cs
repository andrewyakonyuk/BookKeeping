using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace BookKeeping.Core.Storage
{
    public class EventStore : IEventStore
    {
        private readonly IAppendOnlyStore _appendOnlyStore;
        private readonly BinaryFormatter _formatter = new BinaryFormatter();

        public EventStore(IAppendOnlyStore appendOnlyStore)
        {
            _appendOnlyStore = appendOnlyStore;
        }

        public void AppendToStream(IIdentity id, long originalVersion, ICollection<IEvent> events)
        {
            if (!events.Any())
                return;
            var name = IdentityToKey(id);
            using (var memory = new MemoryStream())
            {
                _formatter.Serialize(memory, events.ToArray());
                var data = memory.ToArray();
                try
                {
                    _appendOnlyStore.Append(name, data, originalVersion);
                }
                catch (AppendOnlyStoreConcurrencyException e)
                {
                    // load server events
                    var server = LoadEventStream(id, 0, int.MaxValue);
                    // throw a real problem
                    throw OptimisticConcurrencyException.Create(server.Version, e.ExpectedStreamVersion, id, server.Events);
                }
            }
        }

        public EventStream LoadEventStream(IIdentity id, long skip, int take)
        {
            var name = IdentityToKey(id);
            var records = _appendOnlyStore.ReadRecords(name, skip, take).ToList();
            var stream = new EventStream();

            // TODO: make this lazy somehow?
            foreach (var tapeRecord in records)
            {
                using (var memory = new MemoryStream(tapeRecord.Data))
                {
                    var events = (IEvent[])_formatter.Deserialize(memory);
                    stream.Events.AddRange(events);
                    stream.Version = tapeRecord.Version;
                }
            }
            return stream;
        }

        public EventStream LoadEventStream(IIdentity id)
        {
            return LoadEventStream(id, 0, int.MaxValue);
        }

        private string IdentityToKey(IIdentity id)
        {
            return id == null ? "func" : (id.GetTag() + ":" + id.GetId());
        }
    }
}