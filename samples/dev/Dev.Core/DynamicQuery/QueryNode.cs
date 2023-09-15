using System.Collections.Generic;

namespace Dev.ConsoleApp.DynamicQuery
{
    public class QueryNode
    {
        public QueryNode(string fieldName, QueryOpt opt, object val, QueryNodeType nodeType = QueryNodeType.And)
        {
            Field = new QueryField(fieldName,opt,val);
            NodeType = nodeType;
        }

        public QueryNodeType NodeType { get; set; }
        public QueryField Field { get; set; }
        public LinkedList<QueryNode> Children { get; set; }
        public QueryNode Parent { get; set; }
    }
}
