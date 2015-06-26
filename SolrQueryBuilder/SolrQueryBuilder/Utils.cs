using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SolrQueryBuilder
{
    public static class Utils
    {
        public static string GetPropertyName<T>(Expression<Func<T, object>> expr)
        {
            var result = string.Empty;
            if (expr != null && expr.Body != null)
            {
                if (expr.Body is UnaryExpression)
                {
                    result = ((MemberExpression)((UnaryExpression)expr.Body).Operand).Member.Name;
                }
                else if (expr.Body is MemberExpression)
                {
                    result = ((MemberExpression)expr.Body).Member.Name;
                }
                else if (expr.Body is ParameterExpression)
                {
                    result = ((ParameterExpression)expr.Body).Type.Name;
                }
            }

            return result;
        }

        public static Func<object, string, object> GetMemberFunc<T>()
        {
            Func<object, string, object> result = null;

            var newType = typeof(T);
            var x = System.Linq.Expressions.Expression.Parameter(typeof(object), "x");
            var y = System.Linq.Expressions.Expression.Parameter(typeof(string), "y");
            var hash = System.Linq.Expressions.Expression.Variable(typeof(int), "hash");
            var calHash = System.Linq.Expressions.Expression.Assign(hash, System.Linq.Expressions.Expression.Call(y, typeof(object).GetMethod("GetHashCode")));
            var cases = new List<System.Linq.Expressions.SwitchCase>();
            foreach (var propertyInfo in newType.GetProperties())
            {
                var property = System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Convert(x, newType), propertyInfo.Name);
                var propertyHash = System.Linq.Expressions.Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));

                cases.Add(System.Linq.Expressions.Expression.SwitchCase(System.Linq.Expressions.Expression.Convert(property, typeof(object)), propertyHash));
            }
            var switchEx = System.Linq.Expressions.Expression.Switch(hash, System.Linq.Expressions.Expression.Constant(null), cases.ToArray());
            var methodBody = System.Linq.Expressions.Expression.Block(typeof(object), new[] { hash }, calHash, switchEx);

            result = System.Linq.Expressions.Expression.Lambda<Func<object, string, object>>(methodBody, x, y).Compile();

            return result;
        }
    }
}