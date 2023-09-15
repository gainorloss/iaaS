using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Dev.ConsoleApp.DynamicQuery
{
    public class QueryTableNode
    {
        protected QueryTableNode()
        {
            Children = new LinkedList<QueryTableNode>();
        }

        public QueryTableNode(
            string table,
            IEnumerable<string> fields,
            string key = null,
            QueryJoinType joinType = QueryJoinType.Inner)
            : this()
        {
            var type = TypeUtils.GetTypes(t => t.Name.Equals(table)).FirstOrDefault();
            if (type == null)
                throw new InvalidOperationException($"不存在任何名为{table}的类型");

            Table = type;
            var prop = !string.IsNullOrEmpty(key)
                ? type.GetProperty(key)
                : type.GetProperties().Where(p => p.HasCustomeAttribute<KeyAttribute>(false)).FirstOrDefault();
            if (prop == null)
                throw new InvalidOperationException($"类{table}不存在任何名为{key}的属性");

            Key = prop;

            Fields = fields;
            JoinType = joinType;
        }

        public IEnumerable<string> Fields { get; protected set; }
        public Type Table { get; protected set; }
        public PropertyInfo Key { get; protected set; }
        public QueryJoinType JoinType { get; protected set; }
        public LinkedList<QueryTableNode> Children { get; protected set; }
        public QueryTableNode AddChildren(params QueryTableNode[] children)
        {
            foreach (var child in children)
            {
                Children.AddLast(child);
            }
            return this;
        }
    }
}
