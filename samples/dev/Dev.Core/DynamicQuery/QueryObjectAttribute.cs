using System;

namespace Dev.ConsoleApp.DynamicQuery
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class QueryObjectAttribute
        : Attribute
    {
        public QueryObjectAttribute(string tablesOn)
        {
            TablesOn = tablesOn;
        }
        public string TablesOn { get; }
    }
}
