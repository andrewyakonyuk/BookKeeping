using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BookKeeping.UI
{
    internal static partial class TypeExtensions
    {
        public static T GetCustomAttribute<T>(this Type type, bool inherit)
            where T : Attribute
        {
            return type.GetCustomAttributes<T>(inherit).SingleOrDefault();
        }

        public static T GetCustomeAttribute<T>(this PropertyInfo peoperty, bool inherit = false)
            where T : Attribute
        {
            return (T)peoperty.GetCustomAttributes(typeof(T), inherit).SingleOrDefault();
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this Type type, bool inherited)
            where T : Attribute
        {
            if (type == null) return Enumerable.Empty<T>();
            return type.GetCustomAttributes(typeof(T), inherited).OfType<T>();
        }
    }

}
