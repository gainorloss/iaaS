using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Dev.ConsoleApp.DynamicQuery
{
    internal static class DynamicQueryUtils
    {
        //internal static IEnumerable<string> GetEntityFields(Type queryType)
        //{
        //    if (!queryType.TryGetAttribute<QueryAttribute>(out var attr))
        //        throw new InvalidOperationException(nameof(QueryAttribute));

        //    var fields = new List<string>();
        //    var props = queryType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //    foreach (var prop in props)
        //    {
        //        var column = prop.Name;

        //        var table = attr.TablesOn.Split('=')[0]
        //              .Split('.')[0];
        //        if (prop.TryGetAttribute<ColumnAttribute>(out var columnAttr))
        //            column = columnAttr.Name;

        //        //GetEntityField(table, column);

        //        fields.Add($"{column}={column}");
        //    }

        //    return fields;
        //}

        //private static string GetEntityField(string column)
        //{
        //    var strs = column.Split('.');
        //    var table = strs[0];
        //    var field = strs[1];

        //    var type = TypeUtils.GetTypes(type => type.Name.Equals(table))
        //       .FirstOrDefault();
        //    if (!type.HasCustomeAttribute<TableAttribute>(false) && !type.HasCustomeAttribute<QueryAttribute>(false))
        //        return default;

        //    if (type.HasCustomeAttribute<TableAttribute>(false))
        //    {
        //        var prop = type.GetProperty(field);
        //        if (prop == null)
        //            return default;

        //        return column;
        //    }

        //    var queryAttr = type.GetCustomAttribute<QueryAttribute>(false);
        //    var @default = queryAttr.TablesOn.Split('=')[0]
        //             .Split('.')[0];

        //    if (prop.TryGetAttribute<ColumnAttribute>(out var columnAttr))
        //        column = columnAttr.Name;
        //    column = $"{table}.{propName}";
        //}

        public static bool TryGetAttribute<T>(this ICustomAttributeProvider provider, out T attr)
            where T : Attribute
        {
            attr = provider.GetCustomeAttribute<T>(false);

            if (attr == null)
                return false;

            return true;
        }
    }
}