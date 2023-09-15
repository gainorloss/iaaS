namespace System.Linq.Expressions
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var invokeExpr = Expression.Invoke(right, left.Parameters.Cast<Expression>());
            var andAlso = Expression.AndAlso(left.Body, invokeExpr);

            return Expression.Lambda<Func<T, bool>>(andAlso, left.Parameters);
        }
    }
}
