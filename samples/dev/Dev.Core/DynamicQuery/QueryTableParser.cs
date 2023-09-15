using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Dev.ConsoleApp.DynamicQuery
{
    public class QueryTableParser
    {
        protected IEnumerable<QueryTableNode> Tables { get; }
        public LinkedList<QueryTableNode> Views { get; protected set; }
        public Type ResultType { get; protected set; }

        public QueryTableParser(Type resultType)
        {
            Tables = TypeUtils.GetTypes(t => t.HasCustomeAttribute<TableAttribute>(false))
                  .Select(e =>
                  {
                      var table = e.Name;
                      var props = e.GetProperties();

                      var key = props.FirstOrDefault(prop => prop.HasCustomeAttribute<KeyAttribute>(false));
                      return new QueryTableNode(e.Name, props.Select(p => $"{table}.{p.Name}"), key?.Name);
                  });

            ResultType = resultType;
            Views = UnWrap(resultType);
        }

        private LinkedList<QueryTableNode> UnWrap(Type t)
        {
            var queryAttr = t.GetCustomAttribute<QueryAttribute>();
            var tablesOn = queryAttr.TableOn;
            var tableOns = tablesOn.Split('=');

            var nodes = new LinkedList<QueryTableNode>();
            foreach (var tableOn in tableOns)
            {
                var table = tableOn;
                var key = string.Empty;
                if (tableOn.Contains('.'))
                {
                    var descriptorStrs = tableOn.Split('.');
                    table = descriptorStrs[0];
                    key = descriptorStrs[1];
                }

                var fields = t.GetProperties().Select(p =>
                {
                    var field = $"{table}.{p.Name}";
                    if (p.HasCustomeAttribute<ColumnAttribute>(false))
                    {
                        field = p.GetCustomAttribute<ColumnAttribute>().Name;
                        if (!field.Contains('.'))
                            field = $"{table}.{field}";
                    }
                    return field;
                });
                var node = new QueryTableNode(table, fields, key);

                if (!Tables.Any(t => t.Table.Equals(node.Table)))
                {
                    var type = TypeUtils.GetTypes(t => t.HasCustomeAttribute<QueryAttribute>(false) && t == node.Table)
                        .FirstOrDefault();

                    if (type == null)
                        throw new InvalidOperationException($"动态查询不存在名为{table}的实体类型");

                    var children = UnWrap(type);
                    node.AddChildren(children.ToArray());
                }
                nodes.AddLast(node);
            }

            return nodes;
        }
    }
}
