using ProtoBuf.Meta;
using System;
using System.Collections.Generic;

namespace BookKeeping.Persistance.Storage
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
            var formatter = RuntimeTypeModel.Default.CreateFormatter(type);
            return new Formatter(name, type, formatter.Deserialize, (o, stream) => formatter.Serialize(stream, o));
        }
    }    
}
