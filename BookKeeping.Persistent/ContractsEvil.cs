﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace BookKeeping.Persistent
{
    internal static class ContractEvil
    {
        public sealed class Helper
        {
            readonly Tuple<string, object>[] _attributes;

            public Helper(IEnumerable<object> attributes)
            {
                _attributes = attributes.Select(a => Tuple.Create(a.GetType().Name, a)).ToArray();
            }

            public Maybe<string> GetString(string name, string property)
            {
                if (_attributes.Length == 0)
                    return Maybe<string>.Empty;

                var match = _attributes.FirstOrDefault(t => t.Item1 == name);
                if (null == match)
                    return Maybe<string>.Empty;

                var type = match.Item2.GetType();
                var propertyInfo = type.GetProperty(property);
                if (null == propertyInfo)
                    throw new InvalidOperationException(string.Format("{0}.{1} not found", name, property));
                var result = propertyInfo.GetValue(match.Item2, null) as string;
                if (String.IsNullOrEmpty(result))
                    return Maybe<string>.Empty;

                return result;
            }
        }

        public static string GetContractReference(Type type)
        {
            var attribs = type.GetCustomAttributes(false);
            var helper = new Helper(attribs);


            var s1 = Maybe<string>.Empty;
            var name = s1
                .Combine(() => helper.GetString("ProtoContractAttribute", "Name"))
                .Combine(() => helper.GetString("DataContractAttribute", "Name"))
                .Combine(() => helper.GetString("XmlTypeAttribute", "TypeName"))
                .GetValue(type.Name);

            var ns = s1
                .Combine(() => helper.GetString("DataContractAttribute", "Namespace"))
                .Combine(() => helper.GetString("XmlTypeAttribute", "Namespace"))
                .Convert(s => s.Trim() + "/", "");

            ns = AppendNesting(ns, type);

            return ns + name;
        }

        static string AppendNesting(string ns, Type type)
        {
            var list = new List<string>();
            while (type.IsNested)
            {
                list.Insert(0, type.DeclaringType.Name);
                type = type.DeclaringType;
            }
            if (list.Count == 0)
            {
                return ns;
            }
            var suffix = string.Join("/", Enumerable.ToArray(list)) + "/";
            return ns + suffix;
        }
    }
}
