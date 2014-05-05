using System;
using System.Collections.Generic;
using System.Linq;
using BookKeeping.Core.Domain;
using BookKeeping.Core.Storage;
using BookKeeping.Domain.Contracts;
using ProtoBuf.Meta;

namespace BookKeeping.Infrastructure.Storage
{
    public sealed class DefaultMessageStrategy : MessageStrategyBase, IMessageStrategy
    {
        public DefaultMessageStrategy(ICollection<Type> knownTypes)
            : base(knownTypes)
        {
            RuntimeTypeModel.Default[typeof(DateTimeOffset)].Add("m_dateTime", "m_offsetMinutes");
        }

        protected override Formatter PrepareFormatter(Type type)
        {
            var name = ContractEvil.GetContractReference(type);
            //return new Formatter(name, type, s => JsonSerializer.DeserializeFromStream(type, s), (o, s) =>
            //{
            //    using (var writer = new StreamWriter(s))
            //    {
            //        writer.WriteLine();
            //        writer.WriteLine(JsvFormatter.Format(JsonSerializer.SerializeToString(o, type)));
            //    }

            //});
            var formatter = RuntimeTypeModel.Default.CreateFormatter(type);
            return new Formatter(name, type, formatter.Deserialize, (o, stream) => formatter.Serialize(stream, o));
        }
    }    
}
