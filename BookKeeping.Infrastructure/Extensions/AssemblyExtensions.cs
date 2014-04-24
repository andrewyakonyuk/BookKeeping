using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace BookKeeping.Infrastructure.Extensions
{
    internal static class AssemblyExtensions
    {
        public static IEnumerable<Type> GetTypesImplementing<T>(this Assembly assembly)
        {
            Contract.Requires<ArgumentNullException>(assembly != null, "assembly");
            return assembly.GetExportedTypes().Where(tp => typeof(T).IsAssignableFrom(tp));
        }

        public static IEnumerable<System.Type> GetTypesWithAttribute<T>(this Assembly assembly) where T : Attribute
        {
            Contract.Requires<ArgumentNullException>(assembly != null, "assembly");
            return assembly.GetExportedTypes().Where(a => AttributeExtensions.HasCustomAttribute<T>(a));
        }
    }
}
