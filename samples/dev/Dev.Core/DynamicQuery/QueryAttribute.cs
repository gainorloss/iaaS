using System;

namespace Dev.ConsoleApp.DynamicQuery
{
    public class QueryAttribute
       : Attribute
    {
        public QueryAttribute(string tableOn, QueryJoinType joinType = QueryJoinType.Inner)
        {
            TableOn = tableOn;
            JoinType = joinType;
        }
        public string TableOn { get; set; }
        public QueryJoinType JoinType { get; set; }
    }
}
