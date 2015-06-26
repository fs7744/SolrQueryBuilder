using System;
using System.Linq.Expressions;

namespace SolrQueryBuilder
{
    public static class SolrQ
    {
        public static SolrQ<T> QueryAll<T>() where T : class
        {
            return new SolrQ<T>();
        }

        public static Func<Func<SolrQOperation, Func<string, string>>,SolrQ<T>> Query<T>(Expression<Func<T, object>> expr) where T : class
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