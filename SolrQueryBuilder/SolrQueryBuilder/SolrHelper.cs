using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SolrQueryBuilder
{
    public static class SolrQ
    {
        private static Dictionary<Type, Func<string, string>> m_SolrFiledFunc = new Dictionary<Type, Func<string, string>>();

        internal static Type AttributeType { get; set; }

        private static string GetSolrFieldFunc<T>(string name)
        {
            var type = typeof(T);
            Func<string, string> func = null;
            if (!m_SolrFiledFunc.TryGetValue(type, out func))
            {
                func = Utils.GetSolrFiledNameFunc(type, AttributeType);
                m_SolrFiledFunc.Add(type, func);
            }
            var result = func == null ? name : func(name);
            return string.IsNullOrWhiteSpace(result) ? name : result;
        }

        public static void UpdateSolrFieldAttribute<T>() where T : Attribute
        {
            AttributeType = typeof(T);
        }

        public static void ClearSolrFieldAttribute()
        {
            AttributeType = null;
        }

        public static string GetPropertyName<T>(Expression<Func<T, object>> expr)
        {
            var result = Utils.GetPropertyName(expr);
            result = GetPropertyName<T>(result);
            return result;
        }

        public static string GetPropertyName<T>(string name)
        {
            var result = name;
            if (!string.IsNullOrWhiteSpace(name) && AttributeType != null)
                result = GetSolrFieldFunc<T>(result);
            return result;
        }

        public static SolrQ<T> QueryAll<T>() where T : class
        {
            return new SolrQ<T>();
        }

        public static Func<Func<SolrQOperation, Func<string, string>>, SolrQ<T>> Query<T>(Expression<Func<T, object>> expr) where T : class
        {
            return opFunc => new SolrQ<T>(expr, opFunc);
        }

        public static Func<Func<SolrQOperation, Func<string, string>>, SolrQ<T>> Query<T>(string field) where T : class
        {
            return opFunc => new SolrQ<T>(field, opFunc);
        }

        public static SolrQ<T> QByString<T>(string query) where T : class
        {
            return new SolrQ<T>(query);
        }
    }
}