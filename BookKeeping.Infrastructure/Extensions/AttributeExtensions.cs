using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace BookKeeping.Infrastructure.Extensions
{
    internal static class AttributeExtensions
    {
        public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider provider) where T : Attribute
        {
            return AttributeExtensions.GetCustomAttributes<T>(provider, true);
        }

        public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider provider, bool inherit) where T : Attribute
        {
            Contract.Requires<ArgumentNullException>(provider != null, "provider");
            return provider.GetCustomAttributes(typeof(T), inherit) as T[] ?? new T[0];
        }

        public static T GetSingleAttribute<T>(this ICustomAttributeProvider memberInfo) where T : Attribute
        {
            Contract.Requires<ArgumentNullException>(memberInfo != null, "memberInfo");
            return (T)memberInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();
        }

        public static bool HasCustomAttribute<T>(this ICustomAttributeProvider mi) where T : Attribute
        {
            Contract.Requires<ArgumentNullException>(mi != null, "mi");
            return AttributeExtensions.GetSingleAttribute<T>(mi) != null;
        }
    }
}
