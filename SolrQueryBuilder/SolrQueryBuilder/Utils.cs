using System;
using System.Collections.Generic;
using System.Linq;
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
            var x = Expression.Parameter(typeof(object), "x");
            var y = Expression.Parameter(typeof(string), "y");
            var hash = Expression.Variable(typeof(int), "hash");
            var calHash = Expression.Assign(hash, Expression.Call(y, typeof(object).GetMethod("GetHashCode")));
            var cases = new List<SwitchCase>();
            foreach (var propertyInfo in newType.GetProperties())
            {
                var property = Expression.Property(Expression.Convert(x, newType), propertyInfo.Name);
                var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));

                cases.Add(Expression.SwitchCase(Expression.Convert(property, typeof(object)), propertyHash));
            }
            var switchEx = Expression.Switch(hash, Expression.Constant(null), cases.ToArray());
            var methodBody = Expression.Block(typeof(object), new[] { hash }, calHash, switchEx);

            result = Expression.Lambda<Func<object, string, object>>(methodBody, x, y).Compile();

            return result;
        }

        public static Func<string, string> GetSolrFiledNameFunc(Type type, Type attributeType)
        {
            var x = Expression.Parameter(typeof(string), "x");
            var hash = Expression.Variable(typeof(int), "hash");
            var calHash = Expression.Assign(hash, Expression.Call(x, typeof(string).GetMethod("GetHashCode")));
            var cases = new List<SwitchCase>();
            foreach (var propertyInfo in type.GetProperties())
            {
                var atts = propertyInfo.GetCustomAttributes(attributeType, true);
                if (atts == null || atts.Length == 0)
                    continue;
                var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));
                var property = Expression.Property(Expression.Constant(atts.First(), attributeType), "FieldName");
                cases.Add(Expression.SwitchCase(Expression.Convert(property, typeof(string)), propertyHash));
            }
            var switchEx = Expression.Switch(hash, Expression.Constant(string.Empty), cases.ToArray());
            var methodBody = Expression.Block(typeof(string), new[] { hash }, calHash, switchEx);
            return Expression.Lambda<Func<string, string>>(methodBody, x).Compile();
        }
    }
}