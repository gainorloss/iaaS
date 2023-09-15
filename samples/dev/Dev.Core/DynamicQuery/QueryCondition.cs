using System.Collections.Generic;

namespace Dev.ConsoleApp.DynamicQuery
{
    public class QueryCondition
    {
        public QueryCondition()
        {
            QueryNodes = new LinkedList<QueryNode>();
            OrderByNodes = new HashSet<OrderByNode>();
        }

        public QueryCondition And(string field, QueryOpt opt, object val)
        {
            QueryNodes.AddLast(new QueryNode(field, opt, val, QueryNodeType.And));
            return this;
        }

        public QueryCondition Or(string field, QueryOpt opt, object val)
        {
            QueryNodes.AddLast(new QueryNode(field, opt, val, QueryNodeType.Or));
            return this;
        }

        public QueryCondition OrderBy(string field, Sort orderBy = DynamicQuery.Sort.Asc)
        {
            OrderByNodes.Add(new OrderByNode(field, orderBy));
            return this;
        }

        public LinkedList<QueryNode> QueryNodes { get; protected set; }
        public ICollection<OrderByNode> OrderByNodes { get; protected set; }
    }
}
