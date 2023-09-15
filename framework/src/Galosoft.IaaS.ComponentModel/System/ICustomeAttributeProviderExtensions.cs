using System.Linq;
using System.Reflection;

namespace System
{
    public static class ICustomeAttributeProviderExtensions
    {
        public static T GetCustomeAttribute<T>(this ICustomAttributeProvider customAttributeProvider, bool inherit)
            where T : Attribute
        {
            return customAttributeProvider.GetCustomAttributes(typeof(T), inherit).FirstOrDefault() as T;
        }

        public static bool HasCustomeAttribute<T>(this ICustomAttributeProvider customAttributeProvider, bool inherit)
            where T : Attribute
        {
            return customAttributeProvider.GetCustomAttributes(typeof(T), inherit).Any();
        }
    }
}
