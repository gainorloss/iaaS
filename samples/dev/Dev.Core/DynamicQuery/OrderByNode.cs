namespace Dev.ConsoleApp.DynamicQuery
{
    public class OrderByNode
    {
        public OrderByNode(string fieldName, Sort orderBy = Sort.Asc)
        {
            FieldName = fieldName;
            OrderBy = orderBy;
        }

        public string FieldName { get; protected set; }

        public Sort OrderBy { get; protected set; }
    }
}