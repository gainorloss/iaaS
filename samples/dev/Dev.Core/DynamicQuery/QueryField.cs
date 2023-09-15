namespace Dev.ConsoleApp.DynamicQuery
{
    public class QueryField
    {
        public QueryField(string fieldName,QueryOpt opt,object val)
        {
            FieldName = fieldName;
            Opt = opt;
            Value = val;
        }

        public string FieldName { get; set; }

        public QueryOpt Opt { get; set; }

        public object Value { get; set; }
    }
}
