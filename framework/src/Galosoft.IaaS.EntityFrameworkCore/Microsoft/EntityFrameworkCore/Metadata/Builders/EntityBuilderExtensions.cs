using System;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore.Metadata.Builders
{
    /// <summary>
    /// 
    /// </summary>
    public static class EntityBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="entityTypeBuilder"></param>
        /// <param name="configurer"></param>
        public static void Configure<TAttribute>(this EntityTypeBuilder entityTypeBuilder, Action<EntityTypeBuilder, TAttribute>? configurer = null)
            where TAttribute : Attribute
        {
            var clrType = entityTypeBuilder.Metadata.ClrType;
            if (clrType.HasCustomeAttribute<TAttribute>(false))
            {
                var attr = clrType.GetCustomAttribute<TAttribute>(false);
                configurer?.Invoke(entityTypeBuilder, attr!);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="propertyBuilder"></param>
        /// <param name="configurer"></param>
        public static void Configure<TAttribute>(this PropertyBuilder propertyBuilder, Action<PropertyBuilder, TAttribute>? configurer = null)
            where TAttribute : Attribute
        {
            var prop = propertyBuilder.Metadata.PropertyInfo;
            if (prop.HasCustomeAttribute<TAttribute>(false))
            {
                var attr = prop.GetCustomAttribute<TAttribute>(false);
                configurer?.Invoke(propertyBuilder, attr!);
            }
        }
    }
}
