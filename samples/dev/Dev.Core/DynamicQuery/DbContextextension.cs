using Dev.ConsoleApp.DynamicQuery;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextextension
    {
        public static IQueryable<TResult> Query<TResult>(this DbContext ctx, QueryCondition condition)
        {
            var resultType = typeof(TResult);

            var parser = new QueryTableParser(resultType);

            var query= QueryExpressionBuilder.InnerJoin(ctx, condition, parser);
            return Expression.Lambda(query).Compile().DynamicInvoke() as IQueryable<TResult>;
        }
    }
}

namespace System
{
    public static class TypExtensions
    {
        public static MethodInfo GetGenericMethod(this Type type, string name, params Type[] typeArguments)
        {
            var method = type.GetMethods().FirstOrDefault(m => m.Name.Equals(name) && m.IsGenericMethod);
            var generic = method?.MakeGenericMethod(typeArguments);
            return generic;
        }
    }

    public class QueryExpressionBuilder
    {
        public string PrimaryKey { get; set; }
        public Type SourceType { get; set; }
        public Type ResultType { get; set; }
        public Expression Query { get; set; }
        public ParameterExpression Parameter { get; set; }
        public QueryExpressionBuilder(DbContext ctx, Type sourceType, Type resultType, QueryCondition condition, string primaryKey)
        {
            SourceType = sourceType;
            ResultType = resultType;
            PrimaryKey = primaryKey;

            var dbSetGeneric = typeof(DbContext).GetGenericMethod(nameof(DbContext.Set), sourceType);
            var dbSetCall = Expression.Call(Expression.Constant(ctx), dbSetGeneric);

            var noTrackingGeneric = typeof(EntityFrameworkQueryableExtensions).GetGenericMethod(nameof(EntityFrameworkQueryableExtensions.AsNoTracking), sourceType);
            Query = Expression.Call(noTrackingGeneric, dbSetCall);

            Parameter = Expression.Parameter(sourceType, $"@{sourceType.Name}");
            Where(Parameter, sourceType, condition.QueryNodes);
            OrderBy(Parameter, sourceType, condition.OrderByNodes);
            Select(Parameter, sourceType, resultType);
        }

        public void Where(ParameterExpression para, Type sourceType, LinkedList<QueryNode> queryNodes)
        {
            Expression expAll = null;
            foreach (var node in queryNodes)
            {
                var field = node.Field;
                var property = sourceType.GetProperty(field.FieldName);
                if (property == null)
                    continue;

                var prop = Expression.Property(para, property);

                var constant = Expression.Constant(field.Value, property.PropertyType);

                Expression expr = null;
                MethodInfo method = null;
                switch (field.Opt)
                {
                    case QueryOpt.Contains:
                        method = property.PropertyType.GetMethod(nameof(string.Contains), new[] { typeof(string) });
                        expr = Expression.Call(prop, method, constant);
                        break;
                    case QueryOpt.StartsWith:
                        method = property.PropertyType.GetMethod(nameof(string.StartsWith), new[] { typeof(string) });
                        expr = Expression.Call(prop, method, constant);
                        break;
                    case QueryOpt.EndsWith:
                        method = property.PropertyType.GetMethod(nameof(string.EndsWith), new[] { typeof(string) });
                        expr = Expression.Call(prop, method, constant);
                        break;
                    case QueryOpt.Between:
                        break;
                    case QueryOpt.GreaterThan:
                        expr = Expression.GreaterThan(prop, constant);
                        break;
                    case QueryOpt.GreaterThanAndEqual:
                        expr = Expression.GreaterThanOrEqual(prop, constant);
                        break;
                    case QueryOpt.LessThan:
                        expr = Expression.LessThan(prop, constant);
                        break;
                    case QueryOpt.LessThanAndEqual:
                        expr = Expression.LessThanOrEqual(prop, constant);
                        break;
                    case QueryOpt.In:
                        var vals = field.Value.ToString().Split(" ");
                        Expression temp = null;
                        foreach (var val in vals)
                        {
                            var equal = Expression.Equal(prop, Expression.Constant(val));
                            if (temp == null)
                            {
                                temp = equal;
                                continue;
                            }

                            temp = Expression.OrElse(temp, equal);
                        }
                        expr = temp;
                        break;
                    case QueryOpt.NotEqual:
                        expr = Expression.NotEqual(prop, constant);
                        break;
                    case QueryOpt.Equal:
                    default:
                        expr = Expression.Equal(prop, constant);
                        break;
                }

                expAll = expAll == null
                    ? expr
                    : (node.NodeType == QueryNodeType.And
                        ? Expression.AndAlso(expAll, expr)
                        : Expression.OrElse(expAll, expr));
            }

            if (expAll != null)
            {
                var whereExpr = Expression.Lambda(typeof(Func<,>).MakeGenericType(sourceType, typeof(bool)), expAll, para);

                var whereGeneric = typeof(Queryable).GetGenericMethod(nameof(Queryable.Where), sourceType);
                Query = Expression.Call(whereGeneric, Query, whereExpr);
            }

        }

        public void OrderBy(ParameterExpression para, Type sourceType, IEnumerable<OrderByNode> orderByNodes)
        {
            foreach (var orderByNode in orderByNodes)
            {
                var orderByProperty = sourceType.GetProperty(orderByNode.FieldName);
                if (orderByProperty == null)
                    continue;

                var orderByExpr = Expression.Lambda(Expression.Property(para, orderByProperty), para);

                var orderByType = orderByNode.OrderBy == Sort.Asc
                    ? nameof(Queryable.OrderBy)
                    : nameof(Queryable.OrderByDescending);

                var orderByGeneric = typeof(Queryable).GetGenericMethod(orderByType, sourceType, orderByProperty.PropertyType);
                Query = Expression.Call(orderByGeneric, Query, orderByExpr);
            }
        }

        public void Select(ParameterExpression para, Type sourceType, Type resultType)
        {
            var bindings = new List<MemberBinding>();

            var resultProps = resultType.GetProperties();

            foreach (var prop in resultProps)
            {
                if (sourceType.GetProperty(prop.Name) == null)
                    continue;

                bindings.Add(Expression.Bind(prop, Expression.Property(para, prop.Name)));
            }

            var initExpr = Expression.Lambda(Expression.MemberInit(Expression.New(resultType), bindings), para);

            var selectGeneric = typeof(Queryable).GetGenericMethod(nameof(Queryable.Select), sourceType, resultType);
            Query = Expression.Call(selectGeneric, Query, initExpr);
        }

        public static Expression InnerJoin(DbContext ctx, QueryCondition condition, QueryTableParser parser)
        {
            Expression query = null;
            if (parser.Views.Count < 2)
            {
                var view = parser.Views.First();
                query = new QueryExpressionBuilder(ctx, view.Table, parser.ResultType, condition, view.Key.Name).Query;
            }
            else
            {
                var builders = new List<QueryExpressionBuilder>();
                foreach (var view in parser.Views)
                {
                    var builder = new QueryExpressionBuilder(ctx, view.Table, view.Table, condition, view.Key.Name);
                    builders.Add(builder);
                }

                var typeArguments = parser.Views.Select(v => v.Table).Concat(new[] { typeof(int), parser.ResultType });
                var joinMethod = typeof(Queryable).GetGenericMethod(nameof(Queryable.Join), typeArguments.ToArray());

                var bindings = new List<MemberBinding>();
                var resultProps = parser.ResultType.GetProperties();
                foreach (var prop in resultProps)
                {
                    foreach (var builder in builders)
                    {
                        if (builder.SourceType.GetProperty(prop.Name) != null)
                        {
                            bindings.Add(Expression.Bind(prop, Expression.Property(builder.Parameter, prop.Name)));
                            break;
                        }
                    }
                }

                var paras = new List<Expression>();
                foreach (var builder in builders)
                    paras.Add(builder.Query);

                foreach (var builder in builders)
                    paras.Add(Expression.Lambda(Expression.Property(builder.Parameter, builder.PrimaryKey), builder.Parameter));
                paras.Add(Expression.Lambda(Expression.MemberInit(Expression.New(parser.ResultType), bindings), builders.Select(builder => builder.Parameter).ToArray()));

                query = Expression.Call(joinMethod, paras);
            }
            return query;
        }
    }
}
